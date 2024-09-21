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

using Content.Server.ModularComputer.Devices.Mmio;

namespace Content.Server.ModularComputer.Devices.Tpm;

[RegisterComponent]
[Access(typeof(TpmDeviceSystem))]
public sealed class TpmDeviceComponent : MmioDeviceComponent<TpmDeviceState>
{
    public const int Address = 0x10010000;
    public const int Size = 0x1;

    public override MmioDevice Device { get; } = new("tpm", Address, Size);
}

[Access(typeof(TpmDeviceSystem))]
public sealed class TpmDeviceState : DeviceState
{
}
