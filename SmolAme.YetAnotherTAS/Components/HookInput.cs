using HarmonyLib;
using TAS;
using TAS.Core.Input;
using TAS.Shared;
using UnityEngine;

namespace SmolAme.YetAnotherTAS.Components;

[HarmonyPatch]
public class HookInput {
    private static int currentFrame;
    private static InputFrame previousInput;
    private static InputFrame currentInput;

    public static void SetInputs() {
        InputController controller = Manager.Controller;
        currentFrame = controller.CurrentFrameInTas;
        previousInput = controller.Previous;
        currentInput = controller.Current;
    }

    [EnableRun]
    [DisableRun]
    private static void ResetButtonStates() {
        currentFrame = -1;
        previousInput = null;
        currentInput = null;
    }

    [HarmonyPatch(typeof(Input), nameof(Input.GetAxisRaw))]
    [HarmonyPrefix]
    private static bool InputGetAxisRaw(string axisName, ref float __result) {
        if (!Manager.Running || currentInput == null) {
            return true;
        }

        if (axisName == "Horizontal") {
            if (currentInput.HasActions(Actions.Left)) {
                __result = -1f;
            } else if (currentInput.HasActions(Actions.Right)) {
                __result = 1f;
            }
        } else if (axisName == "Vertical") {
            if (currentInput.HasActions(Actions.Down)) {
                __result = -1f;
            } else if (currentInput.HasActions(Actions.Up)) {
                __result = 1f;
            }
        }

        return false;
    }

    [HarmonyPatch(typeof(Input), nameof(Input.GetButton))]
    [HarmonyPrefix]
    private static bool InputGetButton(string buttonName, ref bool __result) {
        if (!Manager.Running || currentInput == null) {
            return true;
        }

        if (TryGetActions(buttonName, out Actions actions)) {
            __result = currentInput.HasActions(actions);
        }

        return false;
    }

    [HarmonyPatch(typeof(Input), nameof(Input.GetButtonDown))]
    [HarmonyPrefix]
    private static bool InputGetButtonButton(string buttonName, ref bool __result) {
        if (!Manager.Running || currentInput == null) {
            return true;
        }

        if (TryGetActions(buttonName, out Actions actions)) {
            __result = previousInput?.HasActions(actions) != true && currentInput.HasActions(actions);
        }

        return false;
    }

    private static bool TryGetActions(string buttonName, out Actions actions) {
        actions = buttonName switch {
            "Jump" => Actions.Up,
            "PoundButton" => Actions.Down,
            "ExitLevel" => Actions.ExitLevel,
            "Restart" => Actions.RestartLevel,
            "Submit" => Actions.Confirm,
            "Pause" => Actions.Pause,
            _ => Actions.None
        };

        return actions != Actions.None;
    }
}