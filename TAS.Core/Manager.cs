using System;
using System.Collections.Concurrent;
using System.Threading;
using TAS.Core;
using TAS.Core.Hotkey;
using TAS.Core.Input;
using TAS.Core.Input.Commands;
using TAS.Core.Utils;
using TAS.Shared;
using TAS.Shared.Communication.GameToStudio;

namespace TAS;

public static class Manager {
    private static readonly ConcurrentQueue<Action> mainThreadActions = new();

    public static bool Running;
    public static readonly InputController Controller = new();
    public static States LastStates, States, NextStates;
    public static float FastForwardSpeed { get; private set; } = 1f;
    public static bool FastForwarding => FastForwardSpeed > 1f && Running;
    public static bool UltraFastForwarding => FastForwardSpeed >= 10 && Running;
    public static IGame Game { get; private set; }
    public static bool IsLoading => Game.IsLoading;

    static Manager() {
        AttributeUtils.CollectMethods<EnableRunAttribute>();
        AttributeUtils.CollectMethods<DisableRunAttribute>();
        TasCommandAttribute.CollectMethods();
    }

    public static void Init(IGame game) {
        Game = game;
    }

    public static void AddMainThreadAction(Action action) {
        mainThreadActions.Enqueue(action);
    }

    private static void ExecuteMainThreadActions() {
        while (mainThreadActions.TryDequeue(out Action action)) {
            action.Invoke();
        }
    }

    public static void Update() {
        LastStates = States;
        ExecuteMainThreadActions();
        Hotkeys.Update();
        HandleFrameRates();
        CheckToEnable();
        FrameStepping();

        if (States.HasFlag(States.Enable)) {
            Running = true;

            Controller.AdvanceFrame(out bool canPlayback);

            // stop TAS if breakpoint is not placed at the end
            if (Controller.Break && Controller.CanPlayback) {
                States |= States.FrameStep;
                FastForwardSpeed = 1;
            }

            if (!canPlayback) {
                DisableRun();
            }
        } else {
            Running = false;
        }
    }

    private static void HandleFrameRates() {
        FastForwardSpeed = 1;

        if (States.HasFlag(States.Enable) && !States.HasFlag(States.FrameStep)) {
            if (Controller.HasFastForward) {
                FastForwardSpeed = Controller.FastForwardSpeed;
            }

            if (Hotkeys.SlowForward.Check) {
                FastForwardSpeed = Game.SlowForwardSpeed;
            } else if (Hotkeys.FastForward.Check) {
                FastForwardSpeed = Game.FastForwardSpeed;
            } else if (Hotkeys.RightThumbStickX > 0) {
                FastForwardSpeed = Game.FastForwardSpeed;
            } else if (Hotkeys.RightThumbStickX < 0) {
                FastForwardSpeed = Game.SlowForwardSpeed / -Hotkeys.RightThumbStickX;
            }
        }

        Game.SetFrameRate(FastForwardSpeed);
    }

    private static void FrameStepping() {
        if (!IsLoading && States.HasFlag(States.Enable) && (States.HasFlag(States.FrameStep) || Hotkeys.FrameAdvance.Pressed || Hotkeys.PauseResume.Pressed)) {
            if (!States.HasFlag(States.FrameStep)) {
                States |= States.FrameStep;
                return;
            }
            
            bool continueLoop = Hotkeys.FrameAdvance.Pressed;

            while (States.HasFlag(States.Enable)) {
                float rightStickX = Hotkeys.RightThumbStickX;
                if (Hotkeys.SlowForward.Check) {
                    rightStickX = 0.1f;
                } else if (Hotkeys.FastForward.Check) {
                    rightStickX = 0.5f;
                }

                CheckToEnable();

                if (NextStates.HasFlag(States.Enable)) {
                    break;
                }

                if (!continueLoop && Hotkeys.FrameAdvance.Pressed) {
                    States |= States.FrameStep;
                    break;
                } else if (Hotkeys.PauseResume.Pressed) {
                    States &= ~States.FrameStep;
                    break;
                } else if (Math.Abs(rightStickX) > 0f) {
                    States |= States.FrameStep;
                    if (rightStickX < 0) {
                        rightStickX /= 2;
                    }
                    int sleepTime = (int) (1 / Math.Abs(rightStickX) * 17);
                    Thread.Sleep(sleepTime);
                    break;
                }

                continueLoop = Hotkeys.FrameAdvance.Pressed;
                Thread.Sleep(17);
                Hotkeys.Update();
            }
        }
    }

    private static void CheckToEnable() {
        if (Hotkeys.Restart.Pressed) {
            DisableRun();
            NextStates |= States.Enable;
        } else if (Hotkeys.StartStop.Pressed) {
            if (States.HasFlag(States.Enable)) {
                NextStates |= States.Disable;
            } else {
                NextStates |= States.Enable;
            }
        } else if (NextStates.HasFlag(States.Enable)) {
            EnableRun();
        } else if (NextStates.HasFlag(States.Disable)) {
            DisableRun();
        }
    }

    public static void EnableRun() {
        Running = true;
        States |= States.Enable;
        States &= ~States.FrameStep;
        NextStates &= ~States.Enable;
        AttributeUtils.Invoke<EnableRunAttribute>();
        Controller.RefreshInputs(true);
    }

    public static void DisableRun() {
        Running = false;

        LastStates = States = NextStates = States.None;

        AttributeUtils.Invoke<DisableRunAttribute>();
        Controller.Stop();
    }

    public static void DisableRunLater() {
        NextStates |= States.Disable;
    }

    // Called on frame end e.g. Unity component.LateUpdate
    public static void SendStateToStudio() {
        GameInfoMessage gameInfoMessage = new(
            Controller.Previous?.Line ?? -1,
            $"{Controller.CurrentFrameInInput}{Controller.Previous?.RepeatString ?? ""}",
            Controller.CurrentFrameInTas,
            Controller.Inputs.Count,
            States,
            Game.GameInfo,
            Game.LevelName,
            Game.CurrentTime
        );
        CommunicationServer.SendMessage(gameInfoMessage);
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class EnableRunAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class DisableRunAttribute : Attribute { }