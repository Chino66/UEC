using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UEC.Event
{
    public class TypeEventTagCollection
    {
        public Type Type;

        public Dictionary<string /*method name*/, MethodInfo> MethodInfos { get; }

        public TypeEventTagCollection(Type type)
        {
            Type = type;
            MethodInfos = new Dictionary<string, MethodInfo>();
            Collect(type);
        }

        private void Collect(Type type)
        {
            var methodInfos = type.GetMethods(BindingFlags.Instance
                                              | BindingFlags.Static
                                              | BindingFlags.Public
                                              | BindingFlags.NonPublic);
            foreach (var methodInfo in methodInfos)
            {
                var tag = methodInfo.GetCustomAttribute<EventTagAttribute>();
                if (tag == null)
                {
                    continue;
                }

                MethodInfos.Add(methodInfo.Name, methodInfo);
            }
        }

        public object Invoke(object obj, string method, params object[] parameters)
        {
            if (!MethodInfos.TryGetValue(method, out var methodInfo))
            {
                Debug.LogError($"{Type.Name}'s Method \"{method}\" has not [EventTag] Attribute");
                return null;
            }

            return methodInfo.Invoke(obj, parameters);
        }
    }
}