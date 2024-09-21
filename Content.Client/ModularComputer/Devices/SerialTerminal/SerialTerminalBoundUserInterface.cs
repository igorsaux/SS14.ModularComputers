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

using Content.Shared.ModularComputer.Devices.SerialTerminal;
using JetBrains.Annotations;
using Robust.Client.GameObjects;

namespace Content.Client.ModularComputer.Devices.SerialTerminal;

[UsedImplicitly]
public sealed class SerialTerminalBoundUserInterface : BoundUserInterface
{
    private readonly SerialTerminalWindow _window = new();
    
    public SerialTerminalBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
    {
        _window.OnClose += Close;
        _window.MessageEntered += OnMessageEntered;
    }

    private void OnMessageEntered(string message)
    {
        SendMessage(new SerialTerminalSendTextMessage(message));
    }

    protected override void Open()
    {
        base.Open();
        
        _window.OpenCentered();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is not SerialTerminalBoundUserInterfaceState castedState)
            return;
        
        _window.UpdateState(castedState);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        
        if (!disposing)
            return;
        
        _window.Dispose();
    }
}
