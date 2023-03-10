using HarmonyLib;
using SmolAme.YetAnotherTAS.Components.Helpers;
using TAS;
using TAS.Core.Input.Commands;
using UnityEngine;

namespace SmolAme.YetAnotherTAS.Commands;

[HarmonyPatch]
public class TeleportCommand : PluginComponent {
    [TasCommand("Teleport", LegalInMainGame = false)]
    private static void TeleportPosition(string[] args) {
        if (args.Length == 0) {
            return;
        }

        Vector2? position = null;
        Vector2? speed = null;

        if (args.Length >= 2) {
            if (float.TryParse(args[0], out float x) && float.TryParse(args[1], out float y)) {
                position = new Vector2(x, y);
            } else {
                Toast.Show("Teleport Command Failed\nCannot parse position");
                Manager.DisableRunLater();
                return;
            }

            if (args.Length >= 4) {
                if (float.TryParse(args[2], out float speedX) && float.TryParse(args[3], out float speedY)) {
                    speed = new Vector2(speedX, speedY);
                } else {
                    Toast.Show("Teleport Command Failed\nCannot parse speed");
                    Manager.DisableRunLater();
                    return;
                }
            }
        }

        if (position.HasValue) {
            Player.transform.position = position.Value;
        }
        
        if (speed.HasValue) {
            Player.rb.velocity = speed.Value;
        }
    }
}