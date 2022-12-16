using System.Collections.Generic;
using System.Linq;
using SmolAme.YetAnotherTAS.Components;
using TAS;
using TAS.Core;
using TAS.Core.Hotkey;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace SmolAme.YetAnotherTAS;

public class ManagerUpdater : PluginComponent {
    private static readonly PlayerLoopSystem managerUpdateSystem = new() {
        type = typeof(ManagerUpdate),
        updateDelegate = Manager.Update
    };

    private void Awake() {
        Manager.Init(SmolAmeGame.Instance);
        RegisterManagerUpdate();
    }

    private void Update() {
        Hotkeys.AllowKeyboard = Application.isFocused || !CommunicationServer.Connected;
    }

    private void OnDestroy() {
        PlayerLoopSystem playerLoop = PlayerLoop.GetCurrentPlayerLoop();
        for (int index = 0; index < playerLoop.subSystemList.Length; index++) {
            PlayerLoopSystem subSystem = playerLoop.subSystemList[index];
            if (subSystem.type == typeof(PreUpdate)) {
                List<PlayerLoopSystem> preUpdates = subSystem.subSystemList.ToList();
                if (preUpdates.Remove(managerUpdateSystem)) {
                    subSystem.subSystemList = preUpdates.ToArray();
                    playerLoop.subSystemList[index] = subSystem;
                    PlayerLoop.SetPlayerLoop(playerLoop);
                }

                break;
            }
        }
    }

    private static void RegisterManagerUpdate() {
        PlayerLoopSystem playerLoop = PlayerLoop.GetCurrentPlayerLoop();
        for (int index = 0; index < playerLoop.subSystemList.Length; index++) {
            PlayerLoopSystem subSystem = playerLoop.subSystemList[index];
            if (subSystem.type == typeof(PreUpdate)) {
                subSystem.subSystemList = new[] {managerUpdateSystem}.Union(subSystem.subSystemList).ToArray();
                playerLoop.subSystemList[index] = subSystem;
                PlayerLoop.SetPlayerLoop(playerLoop);
                break;
            }
        }
    }

    private struct ManagerUpdate { }
}