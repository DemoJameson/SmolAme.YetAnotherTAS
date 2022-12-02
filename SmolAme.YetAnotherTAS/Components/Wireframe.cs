using System.Collections.Generic;
using BepInEx.Configuration;
using TAS;
using UnityEngine;

namespace SmolAme.YetAnotherTAS.Components;

// modify from https://github.com/ManlyMarco/RuntimeUnityEditor/blob/master/RuntimeUnityEditor/Features/WireframeFeature.cs
public class Wireframe : PluginComponent {
    private static ConfigEntry<bool> wireframeDuringFastForward;
    private static readonly Dictionary<Camera, CameraClearFlags> origFlags = new();
    private static Color? origBackgroundColor;

    private void Awake() {
        wireframeDuringFastForward = Plugin.Instance.Config.Bind("Advanced", "Turn On Wireframe Mode During FastForward", false);
        Camera.onPreRender += PreRender;
        Camera.onPostRender += PostRender;
    }

    private void OnDestroy() {
        Camera.onPreRender -= PreRender;
        Camera.onPostRender -= PostRender;
    }

    private static void PreRender(Camera cam) {
        if (GL.wireframe) {
            return;
        }

        if (!wireframeDuringFastForward.Value || !Manager.FastForwarding) {
            return;
        }

        origBackgroundColor = Camera.main.backgroundColor;
        Camera.main.backgroundColor = Color.black;

        if (!origFlags.ContainsKey(cam)) {
            origFlags.Add(cam, cam.clearFlags);
        }

        cam.clearFlags = CameraClearFlags.Color;
        GL.wireframe = true;
    }

    private static void PostRender(Camera cam) {
        if (origBackgroundColor.HasValue) {
            cam.backgroundColor = origBackgroundColor.Value;
        }

        if (origFlags.TryGetValue(cam, out CameraClearFlags flags)) {
            cam.clearFlags = flags;
            GL.wireframe = false;
        }
    }
}