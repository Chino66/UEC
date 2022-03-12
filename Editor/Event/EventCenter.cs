// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using UnityEngine;
//
// namespace UEC.Event
// {
//     public static class EventCenter
//     {
//         private static Dictionary<Type, TypeEventTagCollection> _collections;
//
//         private static Dictionary<string /*obj id*/, object> _objects;
//
//         static EventCenter()
//         {
//             _collections = new Dictionary<Type, TypeEventTagCollection>();
//             _objects = new Dictionary<string, object>();
//         }
//
//         public static void Clear()
//         {
//             _collections.Clear();
//             _objects.Clear();
//         }
//
//         /// <summary>
//         /// 将这个对象类型所有标记了"EventTag"特性的方法进行注册
//         /// </summary>
//         /// <param name="obj"></param>
//         /// <typeparam name="T"></typeparam>
//         public static void Register(object obj)
//         {
//             ExtractEvent(obj);
//             RegisterObject(obj);
//         }
//
//         private static void RegisterObject(object obj)
//         {
//             // todo 相同类型实例兼容,目前只支持一个类型一个实例
//             var type = obj.GetType();
//
//             if (_objects.ContainsKey(type.Name))
//             {
//                 return;
//             }
//
//             _objects.Add(type.Name, obj);
//         }
//
//         private static TypeEventTagCollection ExtractEvent(object obj)
//         {
//             var type = obj.GetType();
//             if (_collections.TryGetValue(type, out var collection))
//             {
//                 return collection;
//             }
//
//             collection = new TypeEventTagCollection(type);
//
//             _collections.Add(type, collection);
//
//             return collection;
//         }
//
//         public static object SendEvent(string objId, string method, params object[] parameters)
//         {
//             if (_objects.TryGetValue(objId, out var obj))
//             {
//                 return SendEvent(obj, method, parameters);
//             }
//
//             Debug.LogError($"{objId} was not register");
//             return null;
//         }
//
//         private static object SendEvent(object obj, string method, params object[] parameters)
//         {
// //            var type = obj.GetType();
// //            if (!_collections.TryGetValue(type, out var collection))
// //            {
// //                collection = Register(type);
// //            }
//
//             var type = obj.GetType();
//             if (_collections.TryGetValue(type, out var collection))
//             {
//                 return collection.Invoke(obj, method, parameters);
//             }
//             else
//             {
//                 Debug.LogError($"{type.Name} was not register");
//                 return null;
//             }
//         }
//
//         public static void SendRequest(string method, params object[] args)
//         {
//         }
//
//         public static void SendRequest(Request request)
//         {
//         }
//     }
// }