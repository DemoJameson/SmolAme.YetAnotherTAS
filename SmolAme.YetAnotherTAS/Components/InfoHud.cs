using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Configuration;
using TAS;
using TAS.Core.Input;
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
    private static float Margin => optionFontSize.Value * 0.7f;
    private static int Padding => (int) Math.Round(optionFontSize.Value * 0.7f, MidpointRounding.AwayFromZero);

    private void Awake() {
        optionEnabled = Plugin.Instance.Config.Bind("Info", "Info HUD", true);
        optionAlignment = Plugin.Instance.Config.Bind("Info", "Info HUD Alignment", Alignment.TopLeft);
        optionFontSize = Plugin.Instance.Config.Bind("Info", "Info HUD Font Size", 16, new ConfigDescription("", new AcceptableValueRange<int>(8, 40)));
    }

    private void OnGUI() {
        if (!optionEnabled.Value || Event.current?.type != EventType.Repaint) {
            return;
        }

        string tasInput = GetTasInput();
        GUIContent content = new(tasInput + GameInfoHelper.Info);

        style ??= new GUIStyle(GUI.skin.box) {
            font = Font.CreateDynamicFontFromOSFont("Consolas", 16)
        };
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
    
    private static string GetTasInput() {
        StringBuilder result = new();
        InputController controller = Manager.Controller;
        List<InputFrame> inputs = controller.Inputs;
        if (Manager.Running && controller.CurrentFrameInTas >= 0 && controller.CurrentFrameInTas < inputs.Count) {
            InputFrame current = controller.Current;
            if (controller.CurrentFrameInTas >= 1 && current != controller.Previous) {
                current = controller.Previous;
            }

            InputFrame previous = current.Previous;
            InputFrame next = current.Next;

            int maxLine = Math.Max(current.Line, Math.Max(previous?.Line ?? 0, next?.Line ?? 0)) + 1;
            int linePadLeft = maxLine.ToString().Length;

            int maxFrames = Math.Max(current.Frames, Math.Max(previous?.Frames ?? 0, next?.Frames ?? 0));
            int framesPadLeft = maxFrames.ToString().Length;

            string FormatInputFrame(InputFrame inputFrame) {
                return
                    $"{(inputFrame.Line + 1).ToString().PadLeft(linePadLeft)}: {string.Empty.PadLeft(framesPadLeft - inputFrame.Frames.ToString().Length)}{inputFrame}";
            }

            if (previous != null) {
                result.AppendLine(FormatInputFrame(previous));
            }

            string currentStr = FormatInputFrame(current);
            int currentFrameLength = controller.CurrentFrameInInput.ToString().Length;
            int inputWidth = currentStr.Length + currentFrameLength + 2;
            inputWidth = Math.Max(inputWidth, 20);
            result.AppendLine(
                $"{currentStr.PadRight(inputWidth - currentFrameLength)}{controller.CurrentFrameInInput}{current.RepeatString}");

            if (next != null) {
                result.AppendLine(FormatInputFrame(next));
            }

            result.AppendLine();
        }

        return result.ToString();
    }
}