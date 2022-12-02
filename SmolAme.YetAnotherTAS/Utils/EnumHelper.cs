using System;
using System.Collections.Generic;
using System.Linq;

namespace SmolAme.YetAnotherTAS.Utils;

public static class EnumHelpers<T> {
    private static readonly Dictionary<string, T> Dict;
    private static readonly Dictionary<string, T> IgnoreCaseDict;

    static EnumHelpers() {
        Dict = Enum.GetNames(typeof(T)).ToDictionary(x => x, x => (T) Enum.Parse(typeof(T), x), StringComparer.OrdinalIgnoreCase);
        IgnoreCaseDict = Enum.GetNames(typeof(T)).ToDictionary(x => x.ToLower(), x => (T) Enum.Parse(typeof(T), x), StringComparer.OrdinalIgnoreCase);
    }

    public static bool TryParse(string name, out T value, bool ignoreCase = false) {
        if (ignoreCase) {
            return IgnoreCaseDict.TryGetValue(name.ToLower(), out value);
        } else {
            return Dict.TryGetValue(name, out value);
        }
    }
}