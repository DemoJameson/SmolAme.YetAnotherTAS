using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using J2i.Net.XInputWrapper;
using TAS.Shared;

namespace TAS.Core.Hotkey;

public static class Hotkeys {
    private static XboxController xbox;

    public static float RightThumbStickX {
        get {
            float result = xbox.RightThumbStickX / 32768f;
            return Math.Abs(result) < 0.2f ? 0f : result;
        }
    }

    public static Hotkey StartStop { get; private set; }
    public static Hotkey Restart { get; private set; }
    public static Hotkey FastForward { get; private set; }
    public static Hotkey SlowForward { get; private set; }
    public static Hotkey FrameAdvance { get; private set; }
    public static Hotkey PauseResume { get; private set; }
    public static readonly Dictionary<HotkeyID, Hotkey> KeysDict = new();
    public static Dictionary<HotkeyID, List<KeyCodes>> KeysInteractWithStudio { get; private set; }
    public static bool AllowKeyboard = true;
    public static bool AllowController = true;

    static Hotkeys() {
        InputInitialize();
    }

    private static void InputInitialize() {
        xbox = XboxController.RetrieveController(0);
        XboxController.UpdateFrequency = 30;
        XboxController.StartPolling();

        KeysDict.Clear();
        KeysDict[HotkeyID.StartStop] = StartStop = new Hotkey(
            new List<KeyCodes> {KeyCodes.RControlKey}, new List<Buttons> {Buttons.RightStick});
        KeysDict[HotkeyID.Restart] = Restart = new Hotkey(
            new List<KeyCodes> {KeyCodes.Oemplus}, new List<Buttons> {Buttons.LeftStick});
        KeysDict[HotkeyID.FastForward] = FastForward = new Hotkey(
            new List<KeyCodes> {KeyCodes.RShiftKey}, held: true);
        KeysDict[HotkeyID.SlowForward] = SlowForward = new Hotkey(
            new List<KeyCodes> {KeyCodes.RAlt, KeyCodes.RShiftKey}, held: true);
        KeysDict[HotkeyID.FrameAdvance] = FrameAdvance = new Hotkey(
            new List<KeyCodes> {KeyCodes.OemOpenBrackets}, new List<Buttons> {Buttons.DPadUp});
        KeysDict[HotkeyID.PauseResume] = PauseResume = new Hotkey(
            new List<KeyCodes> {KeyCodes.OemCloseBrackets}, new List<Buttons> {Buttons.DPadDown});

        KeysInteractWithStudio = KeysDict.ToDictionary(pair => pair.Key, pair => pair.Value.Keys);
    }

    public static void Update() {
        foreach (Hotkey hotkey in KeysDict.Values) {
            hotkey.Update();
        }
    }

    [DisableRun]
    private static void ReleaseAll() {
        foreach (Hotkey hotkey in KeysDict.Values) {
            hotkey.OverrideCheck = false;
        }
    }

    public class Hotkey {
        public readonly List<KeyCodes> Keys = new();
        public readonly List<Buttons> Buttons = new();
        private readonly bool held;
        private readonly bool keyCombo;
        private DateTime lastPressedTime;
        public bool OverrideCheck;

        public Hotkey(List<KeyCodes> keys = null, List<Buttons> buttons = null, bool held = false, bool keyCombo = true) {
            if (keys != null) {
                Keys.AddRange(keys);
            }

            if (buttons != null) {
                Buttons.AddRange(buttons);
            }

            this.held = held;
            this.keyCombo = keyCombo;
        }

        public bool Check { get; private set; }
        public bool LastCheck { get; private set; }
        public bool Pressed => !LastCheck && Check;

        // note: dont check DoublePressed on render, since unstable DoublePressed response during frame drops
        public bool DoublePressed { get; private set; }
        public bool Released => LastCheck && !Check;

        public void Update(bool updateKey = true, bool updateButton = true) {
            LastCheck = Check;
            bool keyCheck;
            bool buttonCheck;

            if (OverrideCheck) {
                keyCheck = buttonCheck = true;
                if (!held) {
                    OverrideCheck = false;
                }
            } else {
                keyCheck = updateKey && IsKeyDown();
                buttonCheck = updateButton && IsButtonDown();
            }

            Check = keyCheck || buttonCheck;

            if (Pressed) {
                DateTime pressedTime = DateTime.Now;
                DoublePressed = pressedTime.Subtract(lastPressedTime).TotalMilliseconds < 200;
                lastPressedTime = DoublePressed ? default : pressedTime;
            } else {
                DoublePressed = false;
            }
        }

        private bool IsKeyDown() {
            if (Keys.Count == 0 || !AllowKeyboard) {
                return false;
            }

            return keyCombo ? Keys.All(IsKeyDown) : Keys.Any(IsKeyDown);
        }

        private bool IsButtonDown() {
            if (Buttons.Count == 0 || !AllowController) {
                return false;
            }

            return keyCombo ? Buttons.All(IsButtonDown) : Buttons.Any(IsButtonDown);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetAsyncKeyState(KeyCodes keyCode);

        private static bool IsKeyDown(KeyCodes keyCode) {
            return (GetAsyncKeyState(keyCode) & 32768) == 32768;
        }

        private static bool IsButtonDown(Buttons button) {
            return xbox.gamepadStateCurrent.Gamepad.IsButtonPressed((int) button);
        }
    }
}