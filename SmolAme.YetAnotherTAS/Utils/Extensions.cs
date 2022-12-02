using System;
using SmolAme.YetAnotherTAS.Components;
using SmolAme.YetAnotherTAS.Components.Debugs;
using UnityEngine;

namespace SmolAme.YetAnotherTAS.Utils;

internal static class VectorExtensions {
    public static string ToSimpleString(this Vector2 vector2, int decimals = 3) {
        return $"{vector2.x.ToFormattedString(decimals)}, {vector2.y.ToFormattedString(decimals)}";
    }

    public static string ToSimpleString(this Vector3 vector3, int decimals = 3) {
        return $"{vector3.x.ToFormattedString(decimals)}, {vector3.y.ToFormattedString(decimals)}";
    }
}

internal static class NumberExtensions {
    public static string ToFormattedString(this float value, int decimals = 3) {
        return ((double) value).ToFormattedString(decimals);
    }

    public static string ToFormattedString(this double value, int decimals = 3) {
        if (DumpInfo.Dump.Value) {
            return value.ToString();
        } else {
            return value.ToString($"F{decimals}");
        }
    }

    public static int ToCeilingFrames(this float seconds) {
        return (int) Math.Ceiling(seconds * SmolAmeGame.FixedFrameRate);
    }

    public static int ToFloorFrames(this float seconds) {
        return (int) Math.Floor(seconds * SmolAmeGame.FixedFrameRate);
    }
}