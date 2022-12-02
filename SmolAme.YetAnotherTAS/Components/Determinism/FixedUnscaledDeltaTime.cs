using HarmonyLib;
using TAS;
using UnityEngine;

namespace SmolAme.YetAnotherTAS.Components.Determinism;

[HarmonyPatch]
public class FixedUnscaledDeltaTime {
    [HarmonyPatch(typeof(Time), nameof(Time.unscaledDeltaTime), MethodType.Getter)]
    [HarmonyPrefix]
    private static bool TimeUnscaledDeltaTime(ref float __result) {
        if (Manager.Running) {
            __result = Time.deltaTime;
            return false;
        } else {
            return true;
        }
    }
}