using System;
using TAS.Shared;

namespace TAS.Core.Input;

public class InputFrame : InputFrameBase {
    public int Line { get; private set; }
    public InputFrame Previous { get; private set; }
    public InputFrame Next { get; private set; }
    public int RepeatCount { get; set; }
    public int RepeatIndex { get; set; }
    public string RepeatString => RepeatCount > 1 ? $" {RepeatIndex}/{RepeatCount}" : "";

    public override string ToString() {
        return Frames + ToActionsString();
    }

    public static bool TryParse(string line, int studioLine, InputFrame prevInputFrame, out InputFrame inputFrame, int repeatIndex = 0,
        int repeatCount = 0) {
        int index = line.IndexOf(",", StringComparison.Ordinal);
        string framesStr;
        if (index == -1) {
            framesStr = line;
            index = 0;
        } else {
            framesStr = line.Substring(0, index);
        }

        if (!int.TryParse(framesStr, out int frames)) {
            inputFrame = null;
            return false;
        }

        frames = Math.Min(frames, 9999);
        inputFrame = new InputFrame {
            Line = studioLine,
            Frames = frames,
            RepeatIndex = repeatIndex,
            RepeatCount = repeatCount
        };

        while (index < line.Length) {
            char c = char.ToUpper(line[index]);

            if (ActionsUtils.Chars.TryGetValue(c, out Actions actions)) {
                inputFrame.Actions |= actions;
            }

            index++;
        }

        if (prevInputFrame != null) {
            prevInputFrame.Next = inputFrame;
            inputFrame.Previous = prevInputFrame;
        }

        return true;
    }
}