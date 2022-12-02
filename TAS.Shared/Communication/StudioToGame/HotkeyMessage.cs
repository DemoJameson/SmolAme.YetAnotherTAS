using System;

namespace TAS.Shared.Communication.StudioToGame;

[Serializable]
public record struct HotkeyMessage(HotkeyID HotkeyID, bool released) : IStudioToGameMessage {
    public HotkeyID HotkeyID = HotkeyID;
    public bool released = released;
}