﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TAS.Core.Utils;

public static class AttributeUtils {
    private static readonly object[] Parameterless = { };
    private static readonly IDictionary<Type, IEnumerable<MethodInfo>> MethodInfos = new Dictionary<Type, IEnumerable<MethodInfo>>();

    public static void CollectMethods<T>() where T : Attribute {
        MethodInfos[typeof(T)] = Assembly.GetCallingAssembly().GetTypesSafe().SelectMany(type => type
            .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(info => info.GetParameters().Length == 0 && info.GetCustomAttribute<T>() != null));
    }

    public static void Invoke<T>() where T : Attribute {
        if (MethodInfos.ContainsKey(typeof(T))) {
            foreach (MethodInfo methodInfo in MethodInfos[typeof(T)]) {
                methodInfo.Invoke(null, Parameterless);
            }
        }
    }

    public static Type[] GetTypesSafe(this Assembly asm) {
        try {
            return asm.GetTypes();
        } catch (ReflectionTypeLoadException ex) {
            return ex.Types.Where(t => t != null).ToArray();
        }
    }
}