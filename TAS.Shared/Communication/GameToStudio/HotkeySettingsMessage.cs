using System;
using System.Collections.Generic;
using System.Linq;

namespace TAS.Shared.Communication.GameToStudio; 

// net35的枚举传到studio会报错无法解析，所以将枚举改为 byte 和 int
[Serializable]
public record struct HotkeySettingsMessage : IGameToStudioMessage {
    private Dictionary<byte, List<int>> settings = new();
    public Dictionary<HotkeyID, List<KeyCodes>> Settings {
        get => settings.ToDictionary(p => (HotkeyID) p.Key, p => p.Value.Cast<KeyCodes>().ToList());
        set => settings = value.ToDictionary(p => (byte) p.Key, p => p.Value.Cast<int>().ToList());
    }

    public HotkeySettingsMessage(Dictionary<HotkeyID, List<KeyCodes>> settings) {
        Settings = settings;
    }
}