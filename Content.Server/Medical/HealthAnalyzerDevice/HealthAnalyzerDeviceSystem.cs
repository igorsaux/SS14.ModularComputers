//-----------------------------------------------------------------------------
// Copyright 2024 Igor Spichkin
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-----------------------------------------------------------------------------

using Content.Server.Disease;
using Content.Server.ModularComputer;
using Content.Server.ModularComputer.Cpu;
using Content.Server.ModularComputer.Devices.Mmio;
using Content.Server.ModularComputer.Devices.Pci;
using Content.Server.ModularComputer.Devices.Plic;
using Content.Server.NTVM;
using Content.Server.Popups;
using Content.Shared.Damage;
using Content.Shared.Disease.Components;
using Content.Shared.DoAfter;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.MedicalScanner;
using Content.Shared.Mobs.Components;

namespace Content.Server.Medical.HealthAnalyzerDevice;

public sealed class
    HealthAnalyzerDeviceSystem : PciDeviceSystem<HealthAnalyzerDeviceComponent, HealthAnalyzerDeviceState>
{
    [Dependency]
    private readonly DiseaseSystem _disease = default!;

    [Dependency]
    private readonly PopupSystem _popupSystem = default!;

    [Dependency]
    private readonly SharedAudioSystem _audio = default!;

    [Dependency]
    private readonly SharedDoAfterSystem _doAfterSystem = default!;

    [Dependency]
    private readonly PlicDeviceSystem _plic = default!;

    [Dependency]
    private readonly CpuSystem _cpu = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HealthAnalyzerDeviceComponent, AfterInteractEvent>(OnAfterInteract);
        SubscribeLocalEvent<HealthAnalyzerDeviceComponent, HealthAnalyzerDoAfterEvent>(OnDoAfter);
    }

    private void OnAfterInteract(EntityUid uid, HealthAnalyzerDeviceComponent component, AfterInteractEvent args)
    {
       
        if (args.Handled || args.Target == null || !args.CanReach || !HasComp<MobStateComponent>(args.Target))
            return;

        if (component.Motherboard is null || !_cpu.IsRunning(component.Motherboard.Value, null))
            return;
        
        _audio.PlayPvs(component.ScanningBeginSound, uid);

        _doAfterSystem.TryStartDoAfter(
            new DoAfterArgs(args.User, component.ScanDelay, new HealthAnalyzerDoAfterEvent(), uid,
                target: args.Target, used: uid) { BreakOnTargetMove = true, BreakOnUserMove = true, NeedHand = true });

        args.Handled = true;
    }

    private void OnDoAfter(EntityUid uid, HealthAnalyzerDeviceComponent component, DoAfterEvent args)
    {
        if (args.Handled || args.Cancelled || args.Args.Target == null)
            return;

        if (component.Motherboard is null || !_cpu.IsRunning(component.Motherboard.Value, null))
            return;
        
        _audio.PlayPvs(component.ScanningEndSound, args.Args.User);

        UpdateScannedUser(uid, args.Args.Target.Value, component);
        // Below is for the traitor item
        // Piggybacking off another component's doafter is complete CBT so I gave up
        // and put it on the same component
        /*
         * this code is cursed wuuuuuuut
         */
        if (string.IsNullOrEmpty(component.Disease))
        {
            args.Handled = true;
            return;
        }

        _disease.TryAddDisease(args.Args.Target.Value, component.Disease);

        if (args.Args.User == args.Args.Target)
        {
            _popupSystem.PopupEntity(Loc.GetString("disease-scanner-gave-self", ("disease", component.Disease)),
                args.Args.User, args.Args.User);
        }
        else
        {
            _popupSystem.PopupEntity(
                Loc.GetString("disease-scanner-gave-other",
                    ("target", Identity.Entity(args.Args.Target.Value, EntityManager)), ("disease", component.Disease)),
                args.Args.User, args.Args.User);
        }

        args.Handled = true;
    }

    private void UpdateScannedUser(EntityUid uid, EntityUid? target, HealthAnalyzerDeviceComponent? component)
    {
        if (!Resolve(uid, ref component))
            return;

        if(component.Motherboard is null)
            return;

        if (target == null)
            return;

        if (!TryComp<DamageableComponent>(target, out var damageableComponent))
            return;

        var hasDisease = HasComp<DiseasedComponent>(target);
        
        UpdateState(uid, component, state =>
        {
            var damage = damageableComponent.Damage.DamageDict;
            
            state.Damage.Clear();
            
            if (damage.TryGetValue("Asphyxiation", out var asphyxiation))
                state.Damage.Add(DamageTypeId.Asphyxiation, asphyxiation.Float());
            
            if (damage.TryGetValue("Bloodloss", out var bloodloss))
                state.Damage.Add(DamageTypeId.Bloodloss, bloodloss.Float());
            
            if (damage.TryGetValue("Blunt", out var blunt))
                state.Damage.Add(DamageTypeId.Blunt, blunt.Float());
            
            if (damage.TryGetValue("Cellular", out var cellular))
                state.Damage.Add(DamageTypeId.Cellular, cellular.Float());
            
            if (damage.TryGetValue("Caustic", out var caustic))
                state.Damage.Add(DamageTypeId.Caustic, caustic.Float());
            
            if (damage.TryGetValue("Cold", out var cold))
                state.Damage.Add(DamageTypeId.Cold, cold.Float());
            
            if (damage.TryGetValue("Heat", out var heat))
                state.Damage.Add(DamageTypeId.Heat, heat.Float());
            
            if (damage.TryGetValue("Piercing", out var piercing))
                state.Damage.Add(DamageTypeId.Piercing, piercing.Float());
            
            if (damage.TryGetValue("Poison", out var poison))
                state.Damage.Add(DamageTypeId.Poison, poison.Float());
            
            if (damage.TryGetValue("Radiation", out var radiation))
                state.Damage.Add(DamageTypeId.Radiation, radiation.Float());
            
            if (damage.TryGetValue("Shock", out var shock))
                state.Damage.Add(DamageTypeId.Shock, shock.Float());

            if (damage.TryGetValue("Slash", out var slash))
                state.Damage.Add(DamageTypeId.Slash, slash.Float());

            state.HasDisease = hasDisease;
            
            _plic.SendIrq(component.Motherboard.Value, null, null, component.Device.IrqPin);
        });
    }

    protected override bool OnMmioRead(EntityUid uid, Machine machine, MmioDevice device, HealthAnalyzerDeviceState state, BinaryRw data,
        int offset)
    {
        if (offset < HealthAnalyzerDeviceComponent.DamageOffset)
        {
            var register = (DeviceReadRegister)offset;

            switch (register)
            {
                case DeviceReadRegister.HasDisease:
                    data.Write(state.HasDisease);
                    
                    break;
            }
        }
        else
        {
            var damageTypeId = (DamageTypeId)(offset - HealthAnalyzerDeviceComponent.DamageOffset);
        
            if (state.Damage.TryGetValue(damageTypeId, out var damage))
                data.Write(damage);
            else
                data.Write(0.0);
        }

        return true;
    }

    private enum DeviceReadRegister : byte
    {
        HasDisease = 0x0
    }
}
