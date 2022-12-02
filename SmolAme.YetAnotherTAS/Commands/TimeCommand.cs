using SmolAme.YetAnotherTAS.Components;
using SmolAme.YetAnotherTAS.Components.Helpers;
using TAS;
using TAS.Core.Input.Commands;

namespace SmolAme.YetAnotherTAS.Commands;

public class TimeCommand : PluginComponent {
    private static bool? start;

    [TasCommand("Time", AliasNames = new[] {"Time:", "Time："}, CalcChecksum = false)]
    private static void Time() {
        // dummy
    }

    private void Awake() {
        HookHelper.ActiveSceneChanged(() => start = Manager.Running);
    }

    private void Update() {
        if (Manager.Running && start == true && PlayerScript.player.currentState == PlayerState.Victory) {
            start = false;
            MetadataCommand.UpdateAll("Time", command => SmolAmeGame.Instance.CurrentTime);
        }
    }
}