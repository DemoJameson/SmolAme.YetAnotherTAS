using System;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SmolAme.YetAnotherTAS.Components;
using SmolAme.YetAnotherTAS.Components.Helpers;
using TAS;
using TAS.Core.Input.Commands;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SmolAme.YetAnotherTAS.Commands;

[HarmonyPatch]
public class LoadCommand : PluginComponent {
    private static Vector2? position;

    [TasCommand("Load", LegalInMainGame = false)]
    private static void LoadLevel(string[] args) {
        if (args.Length == 0) {
            return;
        }

        if (args.Length >= 3) {
            if (float.TryParse(args[1], out float x) && float.TryParse(args[2], out float y)) {
                position = new Vector2(x, y);
            } else {
                Toast.Show("Load Command Failed\nCannot parse position");
                position = null;
                Manager.DisableRunLater();
                return;
            }
        }

        if (Time.timeScale == 0) {
            Time.timeScale = 1f;
        }

        LoadLevel(args[0], SceneManager.sceneCountInBuildSettings);
    }

    [HarmonyPatch(typeof(PlayerScript), nameof(PlayerScript.Respawn), new Type[] { })]
    [HarmonyILManipulator]
    private static void Respawn(ILContext ilContext) {
        ILCursor ilCursor = new(ilContext);
        if (ilCursor.TryGotoNext(i => i.OpCode == OpCodes.Ldarg_0, i => i.MatchLdfld<PlayerScript>("rb"))) {
            ilCursor.EmitDelegate(() => {
                if (position.HasValue) {
                    Player.transform.position = position.Value;
                    position = null;
                }
            });
        }
    }

    private static void LoadLevel(string sceneName,int numLevels) {
        if (int.TryParse(sceneName, out int index)) {
            index = Mathf.Clamp(index, 0, numLevels - 1);
            LevelLoader.loader.LoadLevel(index);
        } else {
            LevelLoader.loader.LoadLevel(sceneName);
        }
    }
}