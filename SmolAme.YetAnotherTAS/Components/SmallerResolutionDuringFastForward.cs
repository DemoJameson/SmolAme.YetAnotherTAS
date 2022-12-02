using BepInEx.Configuration;
using HarmonyLib;
using TAS;
using UnityEngine;

namespace SmolAme.YetAnotherTAS.Components;

[HarmonyPatch]
public class SmallerResolutionDuringFastForward : PluginComponent {
    private static ConfigEntry<bool> smallerResolutionDuringFastForward;
    private static Resolution? lastResolution;

    [DisableRun]
    private static void ResetResolution() {
        if (lastResolution is { } r) {
            Screen.SetResolution(r.width, r.height, Screen.fullScreenMode, 0);
            lastResolution = null;
        }
    }

    private void Awake() {
        smallerResolutionDuringFastForward = Plugin.Instance.Config.Bind("Advanced", "Smaller Resolution During FastForward", false);
    }

    private void Update() {
        if (!smallerResolutionDuringFastForward.Value) {
            return;
        }

        if (Manager.UltraFastForwarding && lastResolution == null) {
            lastResolution = new Resolution {
                width = Screen.width,
                height = Screen.height
            };

            Screen.SetResolution(320, 200, Screen.fullScreen, 0);
        } else if (!Manager.UltraFastForwarding && lastResolution.HasValue) {
            ResetResolution();
        }
    }
}