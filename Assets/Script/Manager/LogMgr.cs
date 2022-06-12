
/****************************************************
    文件：LogMgr.cs
    功能：日志流程规范  　　白色：常规日志    红色：错误日志   黄色：风险日志或其他  蓝色：需要关注的日志(总是不会被关闭)  log.temp() 临时调试日志用完就删 不可上传
*****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YourNamespace
{
    public sealed class Log
    {
        private Log() { }
        private static List<string> tags = null;
        //要开启哪种级别风险的日志
        private static bool openWhite = true;
        private static bool openBlue = true;
        private static bool openRed = true;
        private static bool openYellow = true;
        //要创建哪种标签日志
        public static void InitTag()
        {
            Debug.unityLogger.logHandler = new CustomLogHandler();

            //核心模块
            OpenTag("Log");
            OpenTag("comm");
            OpenTag("test");
        }
        public static void OnlyProduction()
        {
            openBlue = true;
            openWhite = false;
            openRed = true;
            openYellow = true;
        }
        public static void White(string tag, string message)
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                Debug.LogFormat("{0}--->{1}", tag ,message);
                return;
            }
#endif
            if (!openWhite)
                return;

            if (tags.Contains(tag))
            {
#if UNITY_EDITOR
                Debug.LogFormat("<color=#FFFFFF>{0}---></color>{1}", tag ,message);
#else
                Debug.LogFormat("{0}--->{1}", tag ,message);
#endif
            }
        }
        public static void Yellow(string tag, string message)
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                Debug.LogWarningFormat("{0}--->{1}", tag ,message);
                return;
            }
#endif
            if (!openYellow)
                return;
            if (tags.Contains(tag))
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("<color=#FFFF00>{0}--->[warning]</color>{1}", tag ,message);
#else
                Debug.LogWarningFormat("{0}--->[warning]{1}", tag ,message);
#endif

            }
        }
        public static void Red(string tag, string message)
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                Debug.LogErrorFormat("{0}--->[error]{1}", tag ,message);
                return;
            }
#endif
            if (!openRed)
                return;
            if (tags.Contains(tag))
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("<color=#FF0000>{0}--->[error]</color>{1}", tag ,message);
#else
                Debug.LogErrorFormat("{0}--->[error]{1}", tag ,message);
#endif
            }
        }
        public static void Blue(string tag, string message)
        {

#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                Debug.LogFormat("{0}--->{1}", tag ,message);
                return;
            }
#endif
            if (!openBlue)
                return;
            if (tags.Contains(tag))
            {
#if UNITY_EDITOR
                Debug.LogFormat("<color=#8989B2>{0}---></color>{1}", tag ,message);
#else
                Debug.LogFormat("{0}--->[blue]{1}", tag ,message);
#endif
            }

        }
        public static void Temp(string message)
        {
#if UNITY_EDITOR
            Debug.LogFormat("<color=#0000FF>{0}</color>--->{1}", "Temp" ,message);
#endif         
        }
        private static void OpenTag(string tag)
        {
            if (tags == null)
            {
                tags = new List<string>();
            }
            if (!tags.Contains(tag))
            {
                tags.Add(tag);
            }
        }
    }

    /// <summary>
    /// 自定义日志拦截
    /// </summary>
    public class CustomLogHandler : ILogHandler
    {
        private readonly ILogHandler unityLogHandler;
        public CustomLogHandler()
        {
            this.unityLogHandler = Debug.unityLogger.logHandler;
            Application.logMessageReceived += GetLogCB;
        }

        private void GetLogCB(string condition, string stackTrace, LogType type)
        {
            if(type == LogType.Exception || (type == LogType.Error && !condition.Contains("--->[error]")))
            {
                Debug.LogError("throw Exception!!!");
            }
        }

        void ILogHandler.LogException(Exception exception, UnityEngine.Object context)
        {
            unityLogHandler.LogException(exception,context);
        }

        void ILogHandler.LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            object[] myargs = args;
            try
            {
                if(format == "{0}")
                {
                    if(logType == LogType.Error || logType == LogType.Exception)
#if UNITY_EDITOR
                        format = "<color=#FF0000>{0}--->[error]</color>{1}";
#else
                        format = "{0}--->[error]{1}";
#endif
                    else if(logType == LogType.Warning)
#if UNITY_EDITOR
                        format = "<color=#FFFF00>{0}--->[warning]</color>{1}";
#else
                        format = "{0}--->[warning]{1}";
#endif
                    else
#if UNITY_EDITOR
                        format = "<color=#FFFFFF>{0}---></color>{1}";
#else
                        format = "{0}--->{1}";
#endif

                    object[] newArgs = new object[args.Length + 1];
                    newArgs[0] = "unity";
                    for(int  i = 0;i<args.Length;i++)
                    {
                        newArgs[i+1] = args[i];
                    }
                    myargs = newArgs;
                }
            }
            catch
            {
                myargs = args;
            }
            finally
            {
                unityLogHandler.LogFormat(logType,context,format,myargs);
            }
        }
    }
}
