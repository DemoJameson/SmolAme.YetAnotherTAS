using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;

namespace SmolAme.YetAnotherTAS.Components;

[HarmonyPatch]
public class RunInBackground : PluginComponent {
    private void Update() {
        Application.runInBackground = true;
    }

    [HarmonyPatch(typeof(MainScript), nameof(MainScript.Update))]
    [HarmonyILManipulator]
    private static void MainScriptUpdate(ILContext ilContext) {
        ILCursor ilCursor = new(ilContext);
        if (ilCursor.TryGotoNext(i => i.MatchCall<Application>("get_isFocused"))) {
            ilCursor.Index++;
            ilCursor.Emit(OpCodes.Ldc_I4_1).Emit(OpCodes.Or);
        }
    }
}