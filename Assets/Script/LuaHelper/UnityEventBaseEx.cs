
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using XLua;
namespace YourNamespace
{


    [ReflectionUse]
    public static class UnityEventBaseEx
    {
        public static void ReleaseUnusedListeners(this UnityEventBase unityEventBase)
        {
            unityEventBase.RemoveAllListeners();
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = unityEventBase.GetType();
            MethodInfo method = type.GetMethod("PrepareInvoke", flag);
            method.Invoke(unityEventBase, null);
        }
    }
}
