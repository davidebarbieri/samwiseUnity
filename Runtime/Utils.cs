using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Peevo.Samwise.Unity
{
    public static class Utils
    {
        public static MethodInfo[] FindMethodsWithAttribute<T>() where T : Attribute
        {
#if UNITY_EDITOR
            var extractedTypes = TypeCache.GetMethodsWithAttribute<T>();

            if (extractedTypes.Count == 0)
                return null;
            else
                return extractedTypes.ToArray();
#endif

            if (singleInstanceMethodCache.TryGetValue(typeof(T), out var value))
            {
                return value;
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            List<MethodInfo> methods = new List<MethodInfo>();

            foreach (var assembly in assemblies)
                foreach (var type in assembly.GetTypes())
                    foreach (var method in type.GetMethods())
                        if (method.GetCustomAttribute(typeof(T), true) != null)
                    {
                        methods.Add(method);
                    }

            if (methods.Count > 0)
            {

                return singleInstanceMethodCache[typeof(T)] = methods.ToArray();
            }
            
            singleInstanceMethodCache[typeof(T)] = null;
            return null;
        }

        public static Type FindTypeWithAttribute<T>() where T : Attribute
        {
#if UNITY_EDITOR
            var extractedTypes = TypeCache.GetTypesWithAttribute<T>();

            if (extractedTypes.Count == 0)
                return null;
            else
                return extractedTypes[0];
#endif

            if (singleInstanceCache.TryGetValue(typeof(T), out var value))
            {
                return value;
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
                foreach (var type in assembly.GetTypes())
                    if (type.GetCustomAttribute(typeof(T), true) != null)
                    {
                        singleInstanceCache[typeof(T)] = type;
                        return type;
                    }

            singleInstanceCache[typeof(T)] = null;
            return null;
        }

        static Dictionary<Type, Type> singleInstanceCache = new Dictionary<Type, Type>();
        static Dictionary<Type, MethodInfo[]> singleInstanceMethodCache = new Dictionary<Type, MethodInfo[]>();
    }
}