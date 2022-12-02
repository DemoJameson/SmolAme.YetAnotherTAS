using System;
using HarmonyLib;
using MonoMod.Cil;
using UnityEngine;

namespace SmolAme.YetAnotherTAS.Components.Determinism;

[HarmonyPatch]
public class FixedMusicCycle {
    [HarmonyPatch(typeof(MusicCycleScript), nameof(MusicCycleScript.Update))]
    [HarmonyILManipulator]
    private static void MusicCycleScriptUpdate(ILContext ilContext) {
        ILCursor ilCursor = new(ilContext);
        if (ilCursor.TryGotoNext(i => i.MatchCallvirt<AudioSource>("get_time"))) {
            ilCursor.Index++;
            ilCursor.EmitDelegate<Func<float, float>>(time => MainScript.main.levelTime);
        }
    }
}