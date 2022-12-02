using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TAS.Core.Utils;

internal delegate TReturn GetDelegate<in TInstance, out TReturn>(TInstance instance);

internal static class ReflectionExtensions {
    private const BindingFlags StaticInstanceAnyVisibility =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    private const BindingFlags InstanceAnyVisibilityDeclaredOnly =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private static readonly object[] NullArgs = {null};

    // ReSharper disable UnusedMember.Local
    private record struct MemberKey(Type Type, string Name) {
        public readonly Type Type = Type;
        public readonly string Name = Name;
    }

    private record struct AllMemberKey(Type Type, BindingFlags BindingFlags) {
        public readonly Type Type = Type;
        public readonly BindingFlags BindingFlags = BindingFlags;
    }

    private record struct MethodKey(Type Type, string Name, long Types) {
        public readonly Type Type = Type;
        public readonly string Name = Name;
        public readonly long Types = Types;
    }
    // ReSharper restore UnusedMember.Local

    private static readonly ConcurrentDictionary<MemberKey, FieldInfo> CachedFieldInfos = new();
    private static readonly ConcurrentDictionary<MemberKey, PropertyInfo> CachedPropertyInfos = new();
    private static readonly ConcurrentDictionary<MethodKey, MethodInfo> CachedMethodInfos = new();
    private static readonly ConcurrentDictionary<MemberKey, MethodInfo> CachedGetMethodInfos = new();
    private static readonly ConcurrentDictionary<MemberKey, MethodInfo> CachedSetMethodInfos = new();
    private static readonly ConcurrentDictionary<AllMemberKey, IEnumerable<FieldInfo>> CachedAllFieldInfos = new();
    private static readonly ConcurrentDictionary<AllMemberKey, IEnumerable<PropertyInfo>> CachedAllPropertyInfos = new();

    public static FieldInfo GetFieldInfo(this Type type, string name) {
        var key = new MemberKey(type, name);
        if (CachedFieldInfos.TryGetValue(key, out var result)) {
            return result;
        }

        do {
            result = type.GetField(name, StaticInstanceAnyVisibility);
        } while (result == null && (type = type.BaseType) != null);

        return CachedFieldInfos[key] = result;
    }

    public static PropertyInfo GetPropertyInfo(this Type type, string name) {
        var key = new MemberKey(type, name);
        if (CachedPropertyInfos.TryGetValue(key, out var result)) {
            return result;
        }

        do {
            result = type.GetProperty(name, StaticInstanceAnyVisibility);
        } while (result == null && (type = type.BaseType) != null);

        return CachedPropertyInfos[key] = result;
    }

    public static MethodInfo GetMethodInfo(this Type type, string name, Type[] types = null) {
        var key = new MethodKey(type, name, types.GetCustomHashCode());
        if (CachedMethodInfos.TryGetValue(key, out MethodInfo result)) {
            return result;
        }

        do {
            MethodInfo[] methodInfos = type.GetMethods(StaticInstanceAnyVisibility);
            result = methodInfos.FirstOrDefault(info =>
                info.Name == name && types?.SequenceEqual(info.GetParameters().Select(i => i.ParameterType)) != false);
        } while (result == null && (type = type.BaseType) != null);

        return CachedMethodInfos[key] = result;
    }

    public static MethodInfo GetGetMethod(this Type type, string propertyName) {
        var key = new MemberKey(type, propertyName);
        if (CachedGetMethodInfos.TryGetValue(key, out var result)) {
            return result;
        }

        do {
            result = type.GetPropertyInfo(propertyName)?.GetGetMethod(true);
        } while (result == null && (type = type.BaseType) != null);

        return CachedGetMethodInfos[key] = result;
    }

    public static MethodInfo GetSetMethod(this Type type, string propertyName) {
        var key = new MemberKey(type, propertyName);
        if (CachedSetMethodInfos.TryGetValue(key, out var result)) {
            return result;
        }

        do {
            result = type.GetPropertyInfo(propertyName)?.GetSetMethod(true);
        } while (result == null && (type = type.BaseType) != null);

        return CachedSetMethodInfos[key] = result;
    }

    public static IEnumerable<FieldInfo> GetAllFieldInfos(this Type type, bool includeStatic = false) {
        BindingFlags bindingFlags = InstanceAnyVisibilityDeclaredOnly;
        if (includeStatic) {
            bindingFlags |= BindingFlags.Static;
        }

        var key = new AllMemberKey(type, bindingFlags);
        if (CachedAllFieldInfos.TryGetValue(key, out var result)) {
            return result;
        }

        HashSet<FieldInfo> hashSet = new();
        while (type != null && type.IsSubclassOf(typeof(object))) {
            IEnumerable<FieldInfo> fieldInfos = type.GetFields(bindingFlags);

            foreach (FieldInfo fieldInfo in fieldInfos) {
                if (hashSet.Contains(fieldInfo)) {
                    continue;
                }

                hashSet.Add(fieldInfo);
            }

            type = type.BaseType;
        }

        CachedAllFieldInfos[key] = hashSet;
        return hashSet;
    }

    public static IEnumerable<PropertyInfo> GetAllProperties(this Type type, bool includeStatic = false) {
        BindingFlags bindingFlags = InstanceAnyVisibilityDeclaredOnly;
        if (includeStatic) {
            bindingFlags |= BindingFlags.Static;
        }

        var key = new AllMemberKey(type, bindingFlags);
        if (CachedAllPropertyInfos.TryGetValue(key, out var result)) {
            return result;
        }

        HashSet<PropertyInfo> hashSet = new();
        while (type != null && type.IsSubclassOf(typeof(object))) {
            IEnumerable<PropertyInfo> properties = type.GetProperties(bindingFlags);
            foreach (PropertyInfo fieldInfo in properties) {
                if (hashSet.Contains(fieldInfo)) {
                    continue;
                }

                hashSet.Add(fieldInfo);
            }

            type = type.BaseType;
        }

        CachedAllPropertyInfos[key] = hashSet;
        return hashSet;
    }

    public static T GetFieldValue<T>(this object obj, string name) {
        object result = obj.GetType().GetFieldInfo(name)?.GetValue(obj);
        if (result == null) {
            return default;
        } else {
            return (T) result;
        }
    }

    public static T GetFieldValue<T>(this Type type, string name) {
        object result = type.GetFieldInfo(name)?.GetValue(null);
        if (result == null) {
            return default;
        } else {
            return (T) result;
        }
    }

    public static void SetFieldValue(this object obj, string name, object value) {
        obj.GetType().GetFieldInfo(name)?.SetValue(obj, value);
    }

    public static void SetFieldValue(this Type type, string name, object value) {
        type.GetFieldInfo(name)?.SetValue(null, value);
    }

    public static T GetPropertyValue<T>(this object obj, string name) {
        object result = obj.GetType().GetPropertyInfo(name)?.GetValue(obj, null);
        if (result == null) {
            return default;
        } else {
            return (T) result;
        }
    }

    public static T GetPropertyValue<T>(Type type, string name) {
        object result = type.GetPropertyInfo(name)?.GetValue(null, null);
        if (result == null) {
            return default;
        } else {
            return (T) result;
        }
    }

    public static void SetPropertyValue(this object obj, string name, object value) {
        if (obj.GetType().GetPropertyInfo(name) is {CanWrite: true} propertyInfo) {
            propertyInfo.SetValue(obj, value, null);
        }
    }

    public static void SetPropertyValue(this Type type, string name, object value) {
        if (type.GetPropertyInfo(name) is {CanWrite: true} propertyInfo) {
            propertyInfo.SetValue(null, value, null);
        }
    }

    private static T InvokeMethod<T>(object obj, Type type, string name, params object[] parameters) {
        parameters ??= NullArgs;
        object result = type.GetMethodInfo(name)?.Invoke(obj, parameters);
        if (result == null) {
            return default;
        } else {
            return (T) result;
        }
    }

    public static T InvokeMethod<T>(this object obj, string name, params object[] parameters) {
        return InvokeMethod<T>(obj, obj.GetType(), name, parameters);
    }

    public static T InvokeMethod<T>(this Type type, string name, params object[] parameters) {
        return InvokeMethod<T>(null, type, name, parameters);
    }

    public static void InvokeMethod(this object obj, string name, params object[] parameters) {
        InvokeMethod<object>(obj, obj.GetType(), name, parameters);
    }

    public static void InvokeMethod(this Type type, string name, params object[] parameters) {
        InvokeMethod<object>(null, type, name, parameters);
    }
}

internal static class HashCodeExtensions {
    public static long GetCustomHashCode<T>(this IEnumerable<T> enumerable) {
        if (enumerable == null) {
            return 0;
        }

        unchecked {
            long hash = 17;
            foreach (T item in enumerable) {
                hash = hash * -1521134295 + EqualityComparer<T>.Default.GetHashCode(item);
            }

            return hash;
        }
    }
}

internal static class TypeExtensions {
    public static bool IsSameOrSubclassOf(this Type potentialDescendant, Type potentialBase) {
        return potentialDescendant.IsSubclassOf(potentialBase) || potentialDescendant == potentialBase;
    }

    public static bool IsSameOrSubclassOf(this Type potentialDescendant, params Type[] potentialBases) {
        return potentialBases.Any(potentialDescendant.IsSameOrSubclassOf);
    }

    public static bool IsSimpleType(this Type type) {
        return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime);
    }

    public static bool IsStructType(this Type type) {
        return type.IsValueType && !type.IsEnum && !type.IsPrimitive && type != typeof(decimal);
    }

    public static bool IsConst(this FieldInfo fieldInfo) {
        return fieldInfo.IsLiteral && !fieldInfo.IsInitOnly;
    }
}

internal static class CommonExtensions {
    public static T Apply<T>(this T obj, Action<T> action) {
        action(obj);
        return obj;
    }
}

internal static class StringExtensions {
    private static readonly Regex LineBreakRegex = new(@"\r\n?|\n");

    public static string ReplaceLineBreak(this string text, string replacement) {
        return LineBreakRegex.Replace(text, replacement);
    }

    public static bool IsNullOrEmpty(this string text) {
        return string.IsNullOrEmpty(text);
    }

    public static bool IsNotNullOrEmpty(this string text) {
        return !string.IsNullOrEmpty(text);
    }
}

internal static class EnumerableExtensions {
    public static bool IsEmpty<T>(this IEnumerable<T> enumerable) {
        return !enumerable.Any();
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) {
        return enumerable == null || !enumerable.Any();
    }

    public static bool IsNotEmpty<T>(this IEnumerable<T> enumerable) {
        return !enumerable.IsEmpty();
    }

    public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> enumerable) {
        return !enumerable.IsNullOrEmpty();
    }

    public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int n = 1) {
        var it = source.GetEnumerator();
        bool hasRemainingItems = false;
        var cache = new Queue<T>(n + 1);

        do {
            if (hasRemainingItems = it.MoveNext()) {
                cache.Enqueue(it.Current);
                if (cache.Count > n)
                    yield return cache.Dequeue();
            }
        } while (hasRemainingItems);
    }
}

internal static class ListExtensions {
    public static T GetValueOrDefault<T>(this IList<T> list, int index, T defaultValue = default) {
        return index >= 0 && index < list.Count ? list[index] : defaultValue;
    }
}

internal static class DictionaryExtensions {
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default) {
        return dict.TryGetValue(key, out TValue value) ? value : defaultValue;
    }

    public static TKey LastKeyOrDefault<TKey, TValue>(this SortedDictionary<TKey, TValue> dict) {
        return dict.Count > 0 ? dict.Last().Key : default;
    }

    public static TValue LastValueOrDefault<TKey, TValue>(this SortedDictionary<TKey, TValue> dict) {
        return dict.Count > 0 ? dict.Last().Value : default;
    }
    
    public static void AddRange(this IDictionary dict, IDictionary other) {
        foreach (DictionaryEntry dictionaryEntry in other) {
            dict.Add(dictionaryEntry.Key, dictionaryEntry.Value);
        }
    }
}

internal static class NumberExtensions {
    public static string ToFormattedString(this float value, int decimals) {
        return ((double) value).ToFormattedString(decimals);
    }

    public static string ToFormattedString(this double value, int decimals) {
        return value.ToString($"F{decimals}");
    }
}

internal static class CloneUtil<T> {
    private static readonly Func<T, object> Clone;

    static CloneUtil() {
        MethodInfo cloneMethod = typeof(T).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
        Clone = (Func<T, object>) cloneMethod.CreateDelegate(typeof(Func<T, object>));
    }

    public static T ShallowClone(T obj) => (T) Clone(obj);
}

internal static class CloneUtil {
    public static T ShallowClone<T>(this T obj) => CloneUtil<T>.ShallowClone(obj);
}