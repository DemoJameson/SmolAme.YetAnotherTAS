using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TAS.Shared;

namespace TAS.Studio.Input;

public class InputFrame : InputFrameBase {
    private static readonly Regex EmptyLineRegex = new(@"^\s*$", RegexOptions.Compiled);
    public static readonly Regex CommentSymbolRegex = new(@"^\s*#", RegexOptions.Compiled);
    private static readonly Regex CommentRoomRegex = new(@"^\s*#lvl_", RegexOptions.Compiled);
    private static readonly Regex CommentTimeRegex = new(@"^\s*#(\d+:)?\d{1,2}:\d{2}\.\d{3}", RegexOptions.Compiled);
    public static readonly Regex CommentLineRegex = new(@"^\s*#.*", RegexOptions.Compiled);
    public static readonly Regex BreakpointRegex = new(@"^\s*\*\*\*", RegexOptions.Compiled);
    public static readonly Regex InputFrameRegex = new(@"^(\s*\d+)", RegexOptions.Compiled);

    private static readonly Actions[][] ExclusiveActions = {
        new[] {Actions.Down, Actions.Up},
        new[] {Actions.Left, Actions.Right},
    };

    public string LineText { get; }
    public bool IsInput { get; }
    public bool IsComment { get; }
    public bool IsCommentRoom { get; }
    public bool IsCommentTime { get; }
    public bool IsCommand { get; }
    public bool IsBreakpoint { get; }
    public bool IsEmpty { get; }
    public bool IsEmptyOrZeroFrameInput => IsEmpty || IsInput && Frames == 0;

    public InputFrame(int frames, string actions) : this($"{frames},{actions}") { }

    public InputFrame(string line) {
        LineText = line;

        int index = 0;
        Frames = ReadFrames(line, ref index);
        if (Frames <= 0) {
            if (CommentSymbolRegex.IsMatch(line)) {
                IsComment = true;
                if (CommentRoomRegex.IsMatch(line)) {
                    IsCommentRoom = true;
                } else if (CommentTimeRegex.IsMatch(line)) {
                    IsCommentTime = true;
                }
            } else if (BreakpointRegex.IsMatch(line)) {
                IsBreakpoint = true;
            } else if (InputFrameRegex.IsMatch(line)) {
                IsInput = true;
            } else if (EmptyLineRegex.IsMatch(line)) {
                IsEmpty = true;
            } else {
                IsCommand = true;
            }
        } else {
            IsInput = true;
        }

        if (!IsInput) {
            return;
        }

        while (index < line.Length) {
            char c = char.ToUpper(line[index]);

            if (ActionsUtils.Chars.TryGetValue(c, out Actions actions)) {
                Actions ^= actions;
            }

            index++;
        }
    }

    private int ReadFrames(string line, ref int start) {
        bool foundFrames = false;
        int frames = 0;

        while (start < line.Length) {
            char c = line[start];

            if (!foundFrames) {
                if (char.IsDigit(c)) {
                    foundFrames = true;
                    frames = c ^ 0x30;
                } else if (c != ' ') {
                    break;
                }
            } else if (char.IsDigit(c)) {
                if (frames < 9999) {
                    frames = frames * 10 + (c ^ 0x30);
                } else {
                    frames = 9999;
                }
            } else if (c != ' ') {
                break;
            }

            start++;
        }

        return frames switch {
            < 0 => 0,
            > 9999 => 9999,
            _ => frames
        };
    }

    public override string ToString() {
        return !IsInput ? LineText : Frames.ToString().PadLeft(4, ' ') + ToActionsString();
    }

    public override int GetHashCode() {
        return RuntimeHelpers.GetHashCode(this);
    }

    public virtual bool Equals(InputFrame other) {
        return ReferenceEquals(this, other);
    }

    public bool IsScreenTransition() {
        if (!IsInput || Actions != Actions.None) {
            return false;
        }

        List<InputFrame> inputRecords = Studio.Instance.InputRecords;
        int index = inputRecords.IndexOf(this);
        if (index == -1) {
            return false;
        }

        while (++index < inputRecords.Count) {
            InputFrame next = inputRecords[index];
            if (next.IsEmptyOrZeroFrameInput) {
                continue;
            }

            return next.IsCommentRoom;
        }

        return false;
    }

    public InputFrame Previous(Func<InputFrame, bool> predicate = null) {
        predicate ??= _ => true;
        List<InputFrame> inputRecords = Studio.Instance.InputRecords;
        int index = inputRecords.IndexOf(this);
        if (index == -1) {
            return null;
        }

        while (--index >= 0) {
            InputFrame previous = inputRecords[index];
            if (predicate(previous)) {
                return previous;
            }
        }

        return null;
    }

    public InputFrame Next(Func<InputFrame, bool> predicate = null) {
        predicate ??= _ => true;
        List<InputFrame> inputRecords = Studio.Instance.InputRecords;
        int index = inputRecords.IndexOf(this);
        if (index == -1) {
            return null;
        }

        while (++index < inputRecords.Count) {
            InputFrame next = inputRecords[index];
            if (predicate(next)) {
                return next;
            }
        }

        return null;
    }

    public static void ProcessExclusiveActions(InputFrame oldInput, InputFrame newInput) {
        if (!Settings.Instance.AutoRemoveMutuallyExclusiveActions) {
            return;
        }

        foreach (Actions[] exclusiveActions in ExclusiveActions) {
            foreach (Actions action in exclusiveActions) {
                if (!oldInput.HasActions(action) && newInput.HasActions(action)) {
                    foreach (Actions exclusiveAction in exclusiveActions) {
                        if (exclusiveAction == action) {
                            continue;
                        }

                        newInput.Actions &= ~exclusiveAction;
                    }
                }
            }
        }
    }
}