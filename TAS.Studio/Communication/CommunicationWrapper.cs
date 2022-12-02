using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TAS.Shared;
using TAS.Shared.Communication.GameToStudio;
using TAS.Shared.Communication.StudioToGame;
using TAS.Studio.RichText;
using Char = TAS.Studio.RichText.Char;

namespace TAS.Studio.Communication;

static class CommunicationWrapper {
    public static GameInfoMessage? GameInfo;
    private static Dictionary<HotkeyID, List<KeyCodes>> bindings;
    private static bool fastForwarding;
    private static bool slowForwarding;
    public static bool Forwarding => fastForwarding || slowForwarding;

    [DllImport("User32.dll")]
    private static extern short GetAsyncKeyState(KeyCodes keyCode);

    private static bool IsKeyDown(KeyCodes keyCodes) {
        return (GetAsyncKeyState(keyCodes) & 0x8000) == 0x8000;
    }

    public static void SetBindings(Dictionary<HotkeyID, List<KeyCodes>> newBindings) {
        bindings = newBindings;
    }

    //"wrapper"
    //This doesn't work in release build and i don't particularly care to figure out why.
    public static bool CheckControls(ref Message msg) {
        if (!Settings.Instance.SendHotkeysToGame
            || Environment.OSVersion.Platform == PlatformID.Unix
            || bindings == null
            // check if key is repeated
            || ((int) msg.LParam & 0x40000000) == 0x40000000) {
            return false;
        }

        bool anyPressed = false;
        foreach (HotkeyID hotkeyID in bindings.Keys) {
            List<KeyCodes> keys = bindings[hotkeyID];

            bool pressed = keys.Count > 0 && keys.All(IsKeyDown);

            if (pressed && keys.Count == 1) {
                if (!keys.Contains(KeyCodes.LShiftKey) && IsKeyDown(KeyCodes.LShiftKey)) {
                    pressed = false;
                }

                if (!keys.Contains(KeyCodes.RShiftKey) && IsKeyDown(KeyCodes.RShiftKey)) {
                    pressed = false;
                }

                if (!keys.Contains(KeyCodes.LControlKey) && IsKeyDown(KeyCodes.LControlKey)) {
                    pressed = false;
                }

                if (!keys.Contains(KeyCodes.RControlKey) && IsKeyDown(KeyCodes.RControlKey)) {
                    pressed = false;
                }
            }

            if (pressed) {
                if (hotkeyID == HotkeyID.FastForward) {
                    fastForwarding = true;
                } else if (hotkeyID == HotkeyID.SlowForward) {
                    slowForwarding = true;
                }

                CommunicationClient.SendMessage(new HotkeyMessage(hotkeyID, false));
                anyPressed = true;
            }
        }

        return anyPressed;
    }

    public static void CheckForward() {
        if (fastForwarding) {
            CheckForward(HotkeyID.FastForward, ref fastForwarding);
        } else if (slowForwarding) {
            CheckForward(HotkeyID.SlowForward, ref slowForwarding);
        }
    }

    private static void CheckForward(HotkeyID hotkeyId, ref bool forwarding) {
        if (Environment.OSVersion.Platform == PlatformID.Unix || bindings == null) {
            throw new InvalidOperationException();
        }

        bool pressed;
        if (bindings.ContainsKey(hotkeyId)) {
            List<KeyCodes> keys = bindings[hotkeyId];
            pressed = keys.Count > 0 && keys.All(IsKeyDown);
        } else {
            pressed = false;
        }

        if (!pressed) {
            CommunicationClient.SendMessage(new HotkeyMessage(hotkeyId, true));
            forwarding = false;
        }
    }

    public static void UpdateTexts(Dictionary<int, string> updateLines) {
        RichText.RichText tasText = Studio.Instance.richText;
        foreach (int lineNumber in updateLines.Keys) {
            string lineText = updateLines[lineNumber];
            if (tasText.Lines.Count > lineNumber) {
                Line line = tasText.TextSource[lineNumber];
                line.Clear();
                if (lineText.Length > 0) {
                    line.AddRange(lineText.ToCharArray().Select(c => new Char(c)));
                    Range range = new(tasText, 0, lineNumber, line.Count, lineNumber);
                    range.SetStyle(SyntaxHighlighter.CommandStyle);
                }
            }
        }

        if (updateLines.Count > 0) {
            tasText.NeedRecalc();
        }
    }
}