using System;
using System.Collections.Generic;

namespace TAS.Shared;

[Flags]
public enum Actions {
    None = 0,
    Left = 1 << 0,
    Right = 1 << 1,
    Up = 1 << 2,
    Down = 1 << 3,
    ExitLevel = 1 << 4,
    RestartLevel = 1 << 5,
    Confirm = 1 << 6,
    Pause = 1 << 7,
}

public static class ActionsUtils {
    public static readonly Dictionary<char, Actions> Chars = new() {
        {'L', Actions.Left},
        {'R', Actions.Right},
        {'U', Actions.Up},
        {'D', Actions.Down},
        {'E', Actions.ExitLevel},
        {'S', Actions.RestartLevel},
        {'C', Actions.Confirm},
        {'P', Actions.Pause},
    };
}