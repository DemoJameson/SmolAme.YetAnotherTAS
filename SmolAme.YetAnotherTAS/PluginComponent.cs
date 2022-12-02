using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using SmolAme.YetAnotherTAS.Components;
using SmolAme.YetAnotherTAS.Components.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SmolAme.YetAnotherTAS;

/// <summary>
/// All plugin component will be added in Plugin.Awake();
/// </summary>
public abstract class PluginComponent : MonoBehaviour {
    // ReSharper disable once UnusedMember.Global
    public static ManualLogSource Logger => Plugin.Log;

    private static string currentSceneName;

    public static string CurrentSceneName {
        get => currentSceneName ?? SceneManager.GetActiveScene().name;
        private set => currentSceneName = value;
    }

    public static string PreviousSceneName { get; private set; } = "";

    public static MainScript Main => MainScript.main;
    public static PlayerScript Player => PlayerScript.player;
    public static CamScript Cam => CamScript.camScript;

    /// <summary>
    /// Must be called on plugin.Awake();
    /// </summary>
    /// <param name="gameObject">plugin.gameObject</param>
    public static void Initialize(GameObject gameObject) {
        CurrentSceneName = SceneManager.GetActiveScene().name;
        HookHelper.ActiveSceneChanged((_, nextScene) => {
            PreviousSceneName = CurrentSceneName;
            CurrentSceneName = nextScene.name;
        });

        List<Type> componentTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(PluginComponent))).ToList();
        componentTypes.Sort((type, otherType) => GetPriority(otherType) - GetPriority(type));

        foreach (Type type in componentTypes) {
            gameObject.AddComponent(type);
        }
    }

    private static int GetPriority(Type type) {
        foreach (object attribute in type.GetCustomAttributes(typeof(PluginComponentPriorityAttribute), false)) {
            return ((PluginComponentPriorityAttribute) attribute).Priority;
        }

        return 0;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class PluginComponentPriorityAttribute : Attribute {
    /// <summary>
    /// The higher the priority the earlier it is added to the plugin
    /// </summary>
    public int Priority { get; }

    public PluginComponentPriorityAttribute(int priority) {
        Priority = priority;
    }
}