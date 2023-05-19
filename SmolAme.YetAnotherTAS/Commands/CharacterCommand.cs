using HarmonyLib;
using SmolAme.YetAnotherTAS.Components;
using TAS.Core.Input.Commands;
using TAS.Core.Utils;
using UnityEngine;

namespace SmolAme.YetAnotherTAS.Commands;

public class CharacterCommand {
    private static string GetCharacterName(int index) {
        return index switch {
            1 => "bubba",
            2 => "bloop",
            3 => "hosoinu",
            4 => "onigiri",
            5 => "stache",
            6 => "guraWeird",
            7 => "ushi",
            8 => "shuba",
            9 => "catGura",
            10 => "coco",
            11 => "ollie",
            12 => "reine",
        };
    }

    [TasCommand("Character", LegalInMainGame = false)]
    private static void Character(string[] args) {

        int currentVersion = VersionText.CurrentVersion();

        if (args.IsEmpty()) {
            return;
        }

        if (currentVersion >= 210125 && currentVersion <= 210218) {
            SetNumCharacters(args[0], 1);
        }
        else if (currentVersion > 210218 && currentVersion <= 210325) {
            SetNumCharacters(args[0], 2);
        }
        else if (currentVersion > 210325 && currentVersion <= 210418) {
            SetNumCharacters(args[0], 7);
        }
        else if (currentVersion > 210418 && currentVersion <= 210530) {
            SetNumCharacters(args[0], 9);
        }
        else if (currentVersion > 210530 && currentVersion <= 210612) {
            SetNumCharacters(args[0], 10);
        }
        else {
            SetNumCharacters(args[0], 12);
        }
    }

    private static void SetCharacter(int index) {
        MainScript.currentCharacter = index;
        PlayerPrefs.SetInt("character", MainScript.currentCharacter);
        PlayerScript.player.currentCharacter = MainScript.currentCharacter;
    }

    private static void SetNumCharacters(string args ,int numCharacters) {
        if (int.TryParse(args, out int index)) {
            index = Mathf.Clamp(index, 1, numCharacters) - 1;
            SetCharacter(index);
        }
    }
}