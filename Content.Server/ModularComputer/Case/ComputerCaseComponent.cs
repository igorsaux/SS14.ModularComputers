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
using Robust.Shared.Audio;

namespace Content.Server.ModularComputer.Case;

[RegisterComponent]
[Access(typeof(ComputerCaseSystem))]
public sealed class ComputerCaseComponent : Component
{
    public const float ScrewTime = 1f;

    [DataField("keyboardPressSound", true)]
    public SoundSpecifier? KeyboardPressSound;

    public ItemSlot MotherboardSlot = default!;

    [DataField("motherboardSlot", true, required: true)]
    public string MotherboardSlotId = default!;

    [DataField("mouseClickSound", true)] public SoundSpecifier? MouseClickSound;

    public ItemSlot? PowerCellSlot;

    [DataField("powerCellSlot", true)] public string? PowerCellSlotId;
}
