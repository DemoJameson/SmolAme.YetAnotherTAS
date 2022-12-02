using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SmolAme.YetAnotherTAS.Commands;
using TAS;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SmolAme.YetAnotherTAS.Components.Determinism;

[HarmonyPatch]
public class FixedRandom : PluginComponent {
    private static bool runOrigRandom;

    [HarmonyPatch(typeof(Random), nameof(Random.value), MethodType.Getter)]
    [HarmonyPrefix]
    private static bool RandomValue2(ref float __result) {
        return TryFixedRandom(ref __result, () => Random.value);
    }

    [HarmonyPatch(typeof(Random), nameof(Random.Range), typeof(int), typeof(int))]
    [HarmonyPrefix]
    private static bool RandomRangeInt(int min, int max, ref int __result) {
        return TryFixedRandom(ref __result, () => Random.Range(min, max));
    }

    [HarmonyPatch(typeof(Random), nameof(Random.Range), typeof(float), typeof(float))]
    [HarmonyPrefix]
    private static bool RandomRangeFloat(float min, float max, ref float __result) {
        return TryFixedRandom(ref __result, () => Random.Range(min, max));
    }

    [HarmonyPatch(typeof(Random), nameof(Random.insideUnitSphere), MethodType.Getter)]
    [HarmonyPrefix]
    private static bool RandomInsideUnitSphere(ref Vector3 __result) {
        return TryFixedRandom(ref __result, () => Random.insideUnitSphere);
    }

    [HarmonyPatch(typeof(Random), nameof(Random.onUnitSphere), MethodType.Getter)]
    [HarmonyPrefix]
    private static bool RandomOnUnitSphere(ref Vector3 __result) {
        return TryFixedRandom(ref __result, () => Random.onUnitSphere);
    }

    [HarmonyPatch(typeof(Random), nameof(Random.insideUnitCircle), MethodType.Getter)]
    [HarmonyPrefix]
    private static bool RandomInsideUnitCircle(ref Vector2 __result) {
        return TryFixedRandom(ref __result, () => Random.insideUnitCircle);
    }

    [HarmonyPatch(typeof(Random), nameof(Random.rotation), MethodType.Getter)]
    [HarmonyPrefix]
    private static bool RandomRotation(ref Quaternion __result) {
        return TryFixedRandom(ref __result, () => Random.rotation);
    }

    [HarmonyPatch(typeof(Random), nameof(Random.rotationUniform), MethodType.Getter)]
    [HarmonyPrefix]
    private static bool RandomRotationUniform(ref Quaternion __result) {
        return TryFixedRandom(ref __result, () => Random.rotationUniform);
    }

    private static bool TryFixedRandom<T>(ref T result, Func<T> func) {
        if (Manager.Running && !runOrigRandom) {
            runOrigRandom = true;
            FixedRandomState();
            result = func();
            runOrigRandom = false;
            return false;
        } else {
            return true;
        }
    }

    private static void FixedRandomState(params object[] objects) {
        List<object> seeds = new(objects) {CurrentSceneName + SeedCommand.Seed};
        if (!SmolAmeGame.Instance.IsLoading) {
            seeds.Add(MainScript.main.levelTime);
        }

        Random.InitState(seeds.CombineHashcode());
    }
}

internal static class HashCodeExtension {
    public static int CombineHashcode(this IEnumerable<object> objects) {
        unchecked {
            return objects.Aggregate(17, (current, o) => current * -1521134295 + o.GetHashCode());
        }
    }
}