using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace SmolAme.YetAnotherTAS.Components.Helpers;

public class HookHelper : PluginComponent {
    private static Harmony harmony;
    private static readonly List<UnityAction<Scene, Scene>> SceneChangedActions = new();
    private static readonly List<UnityAction<Scene, LoadSceneMode>> SceneLoadedActions = new();

    public static void ActiveSceneChanged(UnityAction<Scene, Scene> action) {
        SceneChangedActions.Add(action);
        SceneManager.activeSceneChanged += action;
    }

    public static void ActiveSceneChanged(Action action) {
        void UnityAction(Scene _, Scene __) => action();
        SceneChangedActions.Add(UnityAction);
        SceneManager.activeSceneChanged += UnityAction;
    }

    public static void SceneLoaded(Action action) {
        void UnityAction(Scene _, LoadSceneMode __) => action();
        SceneLoadedActions.Add(UnityAction);
        SceneManager.sceneLoaded += UnityAction;
    }

    private void Awake() {
        harmony = Harmony.CreateAndPatchAll(typeof(Plugin).Assembly);
    }

    private void OnDestroy() {
        harmony.UnpatchSelf();

        foreach (UnityAction<Scene, Scene> action in SceneChangedActions) {
            SceneManager.activeSceneChanged -= action;
        }

        SceneChangedActions.Clear();

        foreach (UnityAction<Scene, LoadSceneMode> action in SceneLoadedActions) {
            SceneManager.sceneLoaded -= action;
        }

        SceneLoadedActions.Clear();
    }
}