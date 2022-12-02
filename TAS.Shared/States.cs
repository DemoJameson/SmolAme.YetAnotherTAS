using System;

namespace TAS.Shared;

[Flags]
public enum States : byte {
    None = 0,
    Enable = 1 << 0,
    FrameStep = 1 << 1,
    Disable = 1 << 2
}

#if NET35
public static class StatesExtension {
    public static bool HasFlag(this States states, States other) {
        return (states & other) != 0;
    }
}
#endif