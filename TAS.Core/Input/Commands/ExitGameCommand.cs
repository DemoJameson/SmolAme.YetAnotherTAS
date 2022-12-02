using System;

namespace TAS.Core.Input.Commands;

public static class ExitGameCommand {
    [TasCommand("ExitGame")]
    private static void ExitGame() {
        // need to force close when recording with kkapture, otherwise the game process will still exist
        Environment.Exit(0);
    }
}