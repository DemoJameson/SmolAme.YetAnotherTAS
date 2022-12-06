using SmolAme.YetAnotherTAS.Components;
using TAS;
using TAS.Core.Input.Commands;
using TAS.Core.Utils;
using UnityEngine;

namespace SmolAme.YetAnotherTAS.Commands;

public class FrameRateCommand : PluginComponent {
    [TasCommand("FrameRate", LegalInMainGame = true)]
    private static void SetFrameRate(string[] args) {
        if (args.IsEmpty()) {
            return;
        }

        if (int.TryParse(args[0], out int frameRate)) {
            SmolAmeGame.FixedFrameRate = Mathf.Clamp(frameRate, 1, int.MaxValue);
        }
    }

    [DisableRun]
    private static void Disable() {
        SmolAmeGame.FixedFrameRate = SmolAmeGame.DefaultFixedFrameRate;
    }
}