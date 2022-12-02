using System.Diagnostics;
using BepInEx.Configuration;
using TAS;

namespace SmolAme.YetAnotherTAS.Components.Debugs;

public class Benchmark : PluginComponent {
    private ConfigEntry<bool> benchmark;

    private bool lastRunning;
    private Stopwatch stopwatch;

    private void Awake() {
        benchmark = Plugin.Instance.Config.Bind("Debug", "Benchmark", false, isAdvanced: true);
    }

    private void Update() {
        if (!benchmark.Value) {
            return;
        }

        if (lastRunning != Manager.Running && Manager.Controller.HasFastForward) {
            if (Manager.Running) {
                stopwatch = Stopwatch.StartNew();
            } else if (stopwatch != null) {
                Logger.LogWarning($"Benchmark Result: {stopwatch.ElapsedMilliseconds}ms");
                stopwatch = null;
            }
        }

        if (stopwatch != null) {
            if (Manager.IsLoading && stopwatch.IsRunning) {
                stopwatch.Stop();
            } else if (!Manager.IsLoading && !stopwatch.IsRunning) {
                stopwatch.Start();
            }
        }

        lastRunning = Manager.Running;
    }
}