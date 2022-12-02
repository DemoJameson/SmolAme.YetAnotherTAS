using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TAS;
using TAS.Core.Utils;
using UnityEngine;

namespace SmolAme.YetAnotherTAS.Components.Debugs;

public class DisableRunWhenReload : PluginComponent {
    private Type scriptEngineType;
    private string dllPath;
    private DateTime lastWriteTime;
    private bool reloaded;

    private void Awake() {
        scriptEngineType = Type.GetType("ScriptEngine.ScriptEngine, ScriptEngine");
        dllPath = Path.Combine(Directory.GetParent(Application.dataPath).ToString(), "BepInEx", "scripts", "SmolAme.YetAnotherTAS.dll");
        if (!File.Exists(dllPath) || scriptEngineType == null) {
            Destroy(this);
        }

        Task.Run(() => {
            lastWriteTime = File.GetLastWriteTime(dllPath);
            while (!reloaded) {
                Thread.Sleep(500);
                Debug.LogWarning("Check");
                if (FindObjectOfType(scriptEngineType) is { } scriptEngine && lastWriteTime != File.GetLastWriteTime(dllPath)) {
                    reloaded = true;
                    Manager.DisableRun();
                    scriptEngine.InvokeMethod("ReloadPlugins");
                }
            }
        });
    }
}