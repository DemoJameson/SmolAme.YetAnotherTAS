using System;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SmolAme.YetAnotherTAS.Components;
using SmolAme.YetAnotherTAS.Components.Helpers;
using TAS;
using TAS.Core.Input.Commands;
using UnityEngine;

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
        string sceneName = args[0];
        int currentVersion = VersionText.CurrentVersion();

        if (currentVersion >= 210125 && currentVersion <= 210128) {
            LoadLevel(args[0], 2);
        } else if (currentVersion > 210128 && currentVersion <= 210204) {
            LoadLevel(args[0], 3);
        } else if (currentVersion > 210204 && currentVersion <= 210218) {
            LoadLevel(args[0], 5);
        } else if (currentVersion > 210218 && currentVersion <= 210306) {
            LoadLevel(args[0], 6);
        } else if (currentVersion > 210306 && currentVersion <= 210325) {
            LoadLevel(args[0], 7);
        } else if (currentVersion > 210325 && currentVersion <= 210418) {
            LoadLevel(args[0], 9);
        } else if (currentVersion > 210418 && currentVersion <= 210612) {
            LoadLevel(args[0], 11);
        } else {
            LoadLevel(args[0], 13);
        }

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
            index = Mathf.Clamp(index, 1, numLevels) - 1;
            LevelLoader.loader.LoadLevel(index);
        } else {
            LevelLoader.loader.LoadLevel(sceneName);
        }
    }
}