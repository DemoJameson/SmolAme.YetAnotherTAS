using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TAS.Core.Input.Commands;
using TAS.Core.Utils;
using TAS.Shared;
using Command = TAS.Core.Input.Commands.Command;

namespace TAS.Core.Input;

public class InputController {
    private static readonly string DefaultTasFilePath = Path.Combine(Directory.GetCurrentDirectory(), "game.tas");
    private static string studioTasFilePath = string.Empty;

    public readonly SortedDictionary<int, List<Command>> Commands = new();
    public readonly SortedDictionary<int, FastForward> FastForwards = new();
    public readonly Dictionary<string, List<Comment>> Comments = new();
    public readonly List<InputFrame> Inputs = new();
    public readonly ConcurrentDictionary<string, DateTime> UsedFiles = new();

    private string checksum;
    private int initializationFrameCount;

    public static string StudioTasFilePath {
        get => studioTasFilePath;
        set {
            if (studioTasFilePath == value) {
                return;
            }

            Manager.AddMainThreadAction(() => {
                studioTasFilePath = value;

                Console.WriteLine(studioTasFilePath);
                if (Manager.Running) {
                    Manager.DisableRunLater();
                }

                Manager.Controller.Clear();

                // preload tas file
                Manager.Controller.RefreshInputs(true);
            });
        }
    }

    public static string TasFilePath => string.IsNullOrEmpty(StudioTasFilePath) ? DefaultTasFilePath : StudioTasFilePath;

    // start from 1
    public int CurrentFrameInInput { get; private set; }

    // start from 0
    public int CurrentFrameInTas { get; private set; }

    public InputFrame Previous => Inputs.GetValueOrDefault(CurrentFrameInTas - 1);
    public InputFrame Current => Inputs.GetValueOrDefault(CurrentFrameInTas);
    public InputFrame Next => Inputs.GetValueOrDefault(CurrentFrameInTas + 1);
    public List<Command> CurrentCommands => Commands.GetValueOrDefault(CurrentFrameInTas);
    private bool NeedsReload => UsedFiles.IsEmpty() || UsedFiles.Any(file => File.GetLastWriteTime(file.Key) != file.Value);
    public bool CanPlayback => CurrentFrameInTas < Inputs.Count;
    public bool NeedsToWait => Manager.IsLoading;

    private FastForward CurrentFastForward => FastForwards.FirstOrDefault(pair => pair.Key > CurrentFrameInTas).Value ??
                                              FastForwards.LastOrDefault().Value;

    public bool HasFastForward => CurrentFastForward is { } forward && forward.Frame > CurrentFrameInTas;

    public float FastForwardSpeed => CurrentFastForward is { } forward && forward.Frame > CurrentFrameInTas
        ? forward.Speed
        : 1f;

    public bool Break => CurrentFastForward?.Frame == CurrentFrameInTas;
    private string Checksum => string.IsNullOrEmpty(checksum) ? checksum = CalcChecksum(Inputs.Count - 1) : checksum;

    public void RefreshInputs(bool enableRun) {
        if (enableRun) {
            Stop();
        }

        string lastChecksum = Checksum;
        bool firstRun = UsedFiles.IsEmpty();
        if (NeedsReload) {
            Clear();
            int tryCount = 5;
            while (tryCount > 0) {
                if (ReadFile(TasFilePath)) {
                    if (Manager.NextStates.HasFlag(States.Disable)) {
                        Clear();
                        Manager.DisableRun();
                    } else {
                        ParseFileEnd();
                        if (!firstRun && lastChecksum != Checksum) {
                            MetadataCommand.UpdateRecordCount();
                        }
                    }

                    break;
                } else {
                    System.Threading.Thread.Sleep(50);
                    tryCount--;
                    Clear();
                }
            }

            CurrentFrameInTas = Math.Min(Inputs.Count, CurrentFrameInTas);
        }
    }

    public void Stop() {
        CurrentFrameInInput = 0;
        CurrentFrameInTas = 0;
    }

    public void Clear() {
        initializationFrameCount = 0;
        checksum = string.Empty;
        Inputs.Clear();
        Commands.Clear();
        FastForwards.Clear();
        Comments.Clear();
        UsedFiles.Clear();
        RepeatCommand.Clear();
        ReadCommand.ClearReadCommandStack();
    }

    private void ParseFileEnd() { }

    public void AdvanceFrame(out bool canPlayback) {
        // only refresh inputs when non playback/fast forward
        if (Manager.LastStates != States.Enable || Manager.States != States.Enable || Manager.NextStates != States.None) {
            RefreshInputs(false);
        }

        canPlayback = CanPlayback;

        if (NeedsToWait) {
            return;
        }

        CurrentCommands?.ForEach(command => {
            if (command.Attribute.ExecuteTiming == ExecuteTiming.Runtime && (!EnforceLegalCommand.Enabled || command.Attribute.LegalInMainGame)) {
                command.Invoke();
            }
        });

        if (!CanPlayback) {
            return;
        }

        Manager.Game.SetInputs(Current);

        if (CurrentFrameInInput == 0 || Current.Line == Previous.Line && Current.RepeatIndex == Previous.RepeatIndex) {
            CurrentFrameInInput++;
        } else {
            CurrentFrameInInput = 1;
        }

        CurrentFrameInTas++;
    }

    // studioLine start from 0, startLine start from 1;
    public bool ReadFile(string filePath, int startLine = 0, int endLine = int.MaxValue, int studioLine = 0, int repeatIndex = 0,
        int repeatCount = 0) {
        try {
            if (!File.Exists(filePath)) {
                return false;
            }

            UsedFiles[filePath] = File.GetLastWriteTime(filePath);
            IEnumerable<string> lines = File.ReadAllLines(filePath).Take(endLine);
            ReadLines(lines, filePath, startLine, studioLine, repeatIndex, repeatCount);
            return true;
        } catch (Exception e) {
            Console.WriteLine(e);
            return false;
        }
    }

    public void ReadLines(IEnumerable<string> lines, string filePath, int startLine, int studioLine, int repeatIndex, int repeatCount) {
        int subLine = 0;
        foreach (string readLine in lines) {
            string lineText = readLine.Trim();

            subLine++;
            if (subLine < startLine) {
                continue;
            }

            if (Command.TryParse(this, filePath, subLine, lineText, initializationFrameCount, studioLine, out Command command)
                && command.Is("Play")) {
                // workaround for the play command
                // the play command needs to stop reading the current file when it's done to prevent recursion
                return;
            }

            if (lineText.StartsWith("***")) {
                FastForwards[initializationFrameCount] = new(initializationFrameCount, lineText.Substring(3), studioLine);
            } else if (!lineText.StartsWith("#")) {
                AddFrames(lineText, studioLine, repeatIndex, repeatCount);
            }

            if (filePath == TasFilePath) {
                studioLine++;
            }
        }
    }

    public void AddFrames(string line, int studioLine, int repeatIndex = 0, int repeatCount = 0) {
        if (!InputFrame.TryParse(line, studioLine, Inputs.LastOrDefault(), out InputFrame inputFrame, repeatIndex, repeatCount)) {
            return;
        }

        for (int i = 0; i < inputFrame.Frames; i++) {
            Inputs.Add(inputFrame);
        }

        initializationFrameCount += inputFrame.Frames;
    }

    public InputController Clone() {
        InputController clone = new();

        clone.Inputs.AddRange(Inputs);
        clone.FastForwards.AddRange(FastForwards);

        foreach (string filePath in Comments.Keys) {
            clone.Comments[filePath] = new List<Comment>(Comments[filePath]);
        }

        foreach (int frame in Commands.Keys) {
            clone.Commands[frame] = new List<Command>(Commands[frame]);
        }

        clone.UsedFiles.AddRange(UsedFiles);
        clone.CurrentFrameInTas = CurrentFrameInTas;
        clone.CurrentFrameInInput = CurrentFrameInInput;

        return clone;
    }

    public void CopyFrom(InputController controller) {
        CurrentFrameInInput = controller.CurrentFrameInInput;
        CurrentFrameInTas = controller.CurrentFrameInTas;
    }

    private string CalcChecksum(int toInputFrame) {
        StringBuilder result = new(TasFilePath);
        result.AppendLine();

        int checkInputFrame = 0;

        while (checkInputFrame < toInputFrame) {
            InputFrame currentInput = Inputs[checkInputFrame];
            result.AppendLine(currentInput.ToActionsString());

            if (Commands.GetValueOrDefault(checkInputFrame) is { } commands) {
                foreach (Command command in commands.Where(command => command.Attribute.CalcChecksum)) {
                    result.AppendLine(command.LineText);
                }
            }

            checkInputFrame++;
        }

        return HashHelper.ComputeHash(result.ToString());
    }
}