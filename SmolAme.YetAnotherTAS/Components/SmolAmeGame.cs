using HarmonyLib;
using SmolAme.YetAnotherTAS.Components.Helpers;
using TAS;
using TAS.Core;
using TAS.Core.Input;
using UnityEngine;

namespace SmolAme.YetAnotherTAS.Components;

[HarmonyPatch]
public class SmolAmeGame : PluginComponent, IGame {
    public static SmolAmeGame Instance { get; private set; }
    public static int FixedFrameRate = DefaultFixedFrameRate;
    public static int DefaultFixedFrameRate => 200;
    public string CurrentTime => GameInfoHelper.FormatTime(MainScript.main.levelTime);
    public float FastForwardSpeed => FastForward.DefaultSpeed;
    public float SlowForwardSpeed => 0.1f;
    public string LevelName => CurrentSceneName;
    public ulong FrameCount => (ulong) Time.frameCount;
    public bool IsLoading => loading;
    private static bool loading;

    private void Awake() {
        Instance = this;
        Manager.Init(this);
        HookHelper.SceneLoaded(SceneLoaded);
    }

    public string GameInfo => GameInfoHelper.Info;

    public void SetFrameRate(float multiple) {
        int newFrameRate = (int) (FixedFrameRate * multiple);
        Time.timeScale = Time.timeScale == 0 ? 0 : (float) newFrameRate / FixedFrameRate;
        Time.captureFramerate = newFrameRate;
        Application.targetFrameRate = newFrameRate;
        Time.maximumDeltaTime = Time.fixedDeltaTime;
        QualitySettings.vSyncCount = 0;
    }

    public void SetInputs(InputFrame currentInput) {
        HookInput.SetInputs();
    }

    [HarmonyPatch(typeof(LevelLoader), nameof(LevelLoader.LoadLevel), typeof(int))]
    [HarmonyPatch(typeof(LevelLoader), nameof(LevelLoader.LoadLevel), typeof(string))]
    [HarmonyPostfix]
    private static void LevelLoaderLoadSceneString(LevelLoader __instance) {
        loading = __instance.startLoad || __instance.startLoadString;
    }


    private static void SceneLoaded() {
        loading = false;
    }
}