using System;
using BepInEx.Configuration;
using UnityEngine;

namespace SmolAme.YetAnotherTAS.Components;

public class InfoHud : PluginComponent {
    enum Alignment {
        TopLeft, TopRight, BottomLeft, BottomRight
    }

    private static GUIStyle style;
    private static ConfigEntry<bool> optionEnabled;
    private static ConfigEntry<Alignment> optionAlignment;
    private static ConfigEntry<int> optionFontSize;
    private static float Margin => optionFontSize.Value;
    private static int Padding => optionFontSize.Value;

    private void Awake() {
        optionEnabled = Plugin.Instance.Config.Bind("Info HUD", "Info HUD", true);
        optionAlignment = Plugin.Instance.Config.Bind("Info HUD", "Info HUD Alignment", Alignment.TopRight);
        optionFontSize = Plugin.Instance.Config.Bind("Info HUD", "Info HUD Font Size", 16, new ConfigDescription("", new AcceptableValueRange<int>(8, 40)));
    }

    private void OnGUI() {
        if (!optionEnabled.Value || Event.current?.type != EventType.Repaint) {
            return;
        }

        GUIContent content = new(GameInfoHelper.Info);

        style ??= new GUIStyle(GUI.skin.box);
        style.alignment = TextAnchor.MiddleLeft;
        style.padding = new RectOffset(Padding, Padding, Padding, Padding);
        style.fontSize = optionFontSize.Value;

        Vector2 size = style.CalcSize(content);

        float x;
        float y;
        switch (optionAlignment.Value) {
            case Alignment.TopLeft:
                x = Margin;
                y = Margin;
                break;
            case Alignment.TopRight:
                x = Screen.width - Margin - size.x;
                y = Margin;
                break;
            case Alignment.BottomLeft:
                x = Margin;
                y = Screen.height - Margin - size.y;
                break;
            case Alignment.BottomRight:
                x = Screen.width - Margin - size.x;
                y = Screen.height - Margin - size.y;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        GUI.Box(new Rect(x, y, size.x, size.y), content.text, style);
    }
}