using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BepInEx.Configuration;
using TAS;
using TAS.Core.Utils;
using UnityEngine;

namespace SmolAme.YetAnotherTAS.Components.Debugs;

public class DumpInfo : PluginComponent {
    public static ConfigEntry<bool> Dump { get; private set; }
    private static readonly List<string> infos = new();

    private void Awake() {
        Dump = Plugin.Instance.Config.Bind("Debug", "Dump Info", false, isAdvanced: true);
    }

    private void Update() {
        StartCoroutine(EndOfFrame());
    }

    private IEnumerator EndOfFrame() {
        yield return new WaitForEndOfFrame();

        if (Dump.Value && Manager.Running && !Manager.IsLoading) {
            infos.Add(string.Join("; ", GameInfoHelper.Info.Split('\n')));
        }
    }

    [DisableRun]
    private static void Write() {
        if (Dump.Value && infos.IsNotEmpty()) {
            File.WriteAllLines($"dump_{DateTime.Now.ToFileTime()}.txt", infos);
        }

        infos.Clear();
    }
}