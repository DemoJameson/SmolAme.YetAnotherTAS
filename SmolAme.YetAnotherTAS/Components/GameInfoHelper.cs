using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using HarmonyLib;
using SmolAme.YetAnotherTAS.Commands;
using SmolAme.YetAnotherTAS.Components.Helpers;
using SmolAme.YetAnotherTAS.Utils;
using UnityEngine;

namespace SmolAme.YetAnotherTAS.Components;

[HarmonyPatch]
public class GameInfoHelper : PluginComponent {
    private static ConfigEntry<int> decimalsConfig;
    
    private static Vector3? lastPlayerPosition;
    private static string lastLevelName;
    private static string lastTime;
    private static string lastInfo;
    private static readonly List<string> infos = new();
    private static readonly List<string> statuses = new();
    private static string seed => SeedCommand.Seed == "" ? "" : $" Seed: {SeedCommand.Seed}";

    private void Awake() {
        decimalsConfig = Plugin.Instance.Config.Bind("General", "Info Decimals", 3, new ConfigDescription("", new AcceptableValueRange<int>(1, 99)));

        HookHelper.ActiveSceneChanged(() => {
            lastPlayerPosition = null;
            lastLevelName = null;
            lastTime = null;
            lastInfo = null;
        });
    }

    public static string Info {
        get {
            SmolAmeGame game = SmolAmeGame.Instance;

            if (lastLevelName == CurrentSceneName && lastTime == game.CurrentTime) {
                lastLevelName = CurrentSceneName;
                return lastInfo;
            }

            if (PlayerScript.player is { } player) {
                infos.Clear();
                statuses.Clear();

                Vector3 position = player.transform.position;
                infos.Add($"Pos:   {position.ToSimpleString(decimalsConfig.Value)}");
                infos.Add($"Speed: {(player.rb.velocity).ToSimpleString(decimalsConfig.Value)}");

                lastPlayerPosition ??= position;
                infos.Add($"Vel:   {((position - lastPlayerPosition.Value) * SmolAmeGame.FixedFrameRate).ToSimpleString(decimalsConfig.Value)}");
                lastPlayerPosition = position;

                infos.Add($"Gravity: {player.gravDir.ToSimpleString(decimalsConfig.Value)} ");

                if (player.grounded > 0f || player.canJump) {
                    statuses.Add("Jump");
                }

                if (player.grounded <= 0f || player.yV > 0f || player.canGroundPound) {
                    statuses.Add("GP");
                }

                infos.Add(statuses.Join(delimiter: " "));
                infos.Add($"{game.CurrentTime}{seed}");
                infos.Add($"[{game.LevelName}]");

                lastTime = game.CurrentTime;
                return lastInfo = infos.Join(delimiter: "\n");
            } else {
                return $"[{game.LevelName}]";
            }
        }
    }

    public static string FormatTime(float time) {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        string formatted =
            $"{timeSpan.Minutes.ToString().PadLeft(2, '0')}:{timeSpan.Seconds.ToString().PadLeft(2, '0')}.{timeSpan.Milliseconds.ToString().PadLeft(3, '0')}";
        return $"{formatted}({time.ToCeilingFrames()})";
    }
}