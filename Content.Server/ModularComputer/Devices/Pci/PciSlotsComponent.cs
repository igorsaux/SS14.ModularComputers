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

using Content.Shared.Containers.ItemSlots;
using Content.Shared.Whitelist;

namespace Content.Server.ModularComputer.Devices.Pci;

[RegisterComponent]
[Access(typeof(PciSlotsSystem))]
public sealed class PciSlotsComponent : Component
{
    public readonly List<ItemSlot> PciSlots = new();

    [DataField("count", true, required: true)]
    public int Count;

    [DataField("defaultBlacklist", true)] public EntityWhitelist? DefaultBlacklist;

    [DataField("defaultWhitelist", true)] public EntityWhitelist? DefaultWhitelist;

    [ViewVariables] public bool IsLocked = true;

    [DataField("overrides", true)] public Dictionary<int, ItemSlot> Overrides = new();
}
