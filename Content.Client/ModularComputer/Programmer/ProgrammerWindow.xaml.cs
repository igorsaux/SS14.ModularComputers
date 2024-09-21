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

using Content.Client.Message;
using Content.Client.UserInterface.Controls;
using Content.Shared.ModularComputer.Programmer;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.ModularComputer.Programmer;

[GenerateTypedNameReferences]
public sealed partial class ProgrammerWindow : FancyWindow
{
    public event Action? UploadBootromPressed;
    
    public ProgrammerWindow()
    {
        RobustXamlLoader.Load(this);

        UploadBootromButton.OnPressed += _ => UploadBootromPressed?.Invoke();
    }

    public void UpdateState(ProgrammerBoundUserInterfaceState state)
    {
        StatusLabel.SetMarkup(Loc.GetString($"modular-computers-programmer-ui-state-{state.State.ToString()}"));
        LimitsLabel.SetMarkup(Loc.GetString("modular-computers-programmer-ui-limits", ("mb", state.Limit / 1_000_000)));
    }

    public void SetButtonDisabledState(bool state)
    {
        UploadBootromButton.Disabled = state;
    }
}
