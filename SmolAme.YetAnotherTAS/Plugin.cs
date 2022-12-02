using System.Collections;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SmolAme.YetAnotherTAS.Components;
using TAS;
using TAS.Core;
using TAS.Core.Hotkey;
using UnityEngine;

namespace SmolAme.YetAnotherTAS;

[HarmonyPatch]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Smol Ame.exe")]
public class Plugin : BaseUnityPlugin {
    public static Plugin Instance { get; private set; }
    public static ManualLogSource Log => Instance.Logger;

    private void Awake() {
        Instance = this;
        PluginComponent.Initialize(gameObject);
        Manager.Init(SmolAmeGame.Instance);
    }

    private void Update() {
        StartCoroutine(EndOfFrame());
    }

    private IEnumerator EndOfFrame() {
        yield return new WaitForEndOfFrame();
        Manager.Update();
        Manager.SendStateToStudio();
        Hotkeys.AllowKeyboard = Application.isFocused || !CommunicationServer.Connected;
    }
}