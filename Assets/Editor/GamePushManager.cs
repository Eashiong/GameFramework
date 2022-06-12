//项目管理工具

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using System;
using System.IO;

namespace YourNamespace.Tool
{
    public class GamePushManager : EditorWindow
    {

        static private BuildTarget buildTarget = BuildTarget.Android;

        static private string resVersion = "1.0.000";
        static private string bundleVersion = "1.0.000";

        static private AssetConfig localAssetConfig = AssetConfig.Res;
        static private NetConfig localNetConfig = NetConfig.Test;

        static string log = "";
        private bool foldoutChanged;
        private bool versionFoldout;
        private bool localServerFoldout;

        private bool abFoldout;

        [MenuItem("Tools/游戏管理")]
        private static void OpenWindow()
        {
            Log.InitTag();
            GamePushManager gameMgrWin = EditorWindow.GetWindow(typeof(GamePushManager), true, "游戏管理", true) as GamePushManager;
            gameMgrWin.minSize = new Vector2(400, 100);
            gameMgrWin.maxSize = new Vector2(400, 100);
            gameMgrWin.foldoutChanged = true;
        }

        void OnEnable()
        {
            buildTarget = EditorUserBuildSettings.activeBuildTarget;

            resVersion = GamePushAPI.GetResVersion();
            bundleVersion = PlayerSettings.bundleVersion;

            localAssetConfig = GamePushAPI.GetAssetConfig();
            localNetConfig = GamePushAPI.GetNetConfig();
        }

        void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            GUILayout.Label("当前平台  " + buildTarget.ToString(),EditorStyles.whiteLargeLabel);
            GUILayout.Space(5);
            bool buildTargetSupport = false;
            if (buildTarget != BuildTarget.Android && buildTarget != BuildTarget.iOS)
            {
                EditorGUILayout.HelpBox("请切换到IOS或者Android !!!",MessageType.Error);
            }
            else
            {
                buildTargetSupport = true;
            }
            GUILayout.EndVertical();
            using(new EditorGUI.DisabledScope(!buildTargetSupport))
            {
                GUISeparator();
                DrawBuildPlayerGUI();
                GUISeparator();
                
                GUISeparator();
                DrawConfigGUI();

                GUISeparator();
                DrawLocalServerGUI();

                GUISeparator();
                DrawAssetBundlesGUIAsync();
                
            }
            if(foldoutChanged && Event.current.type == EventType.Repaint)
            {
                foldoutChanged = false;
                
                Rect r = GUILayoutUtility.GetLastRect();
                this.minSize = new Vector2(400,r.y + 100);
                this.maxSize = new Vector2(400,r.y + 100);
            }
        }

        #region 版本配置GUI
        void DrawConfigGUI()
        {
            EditorGUI.BeginChangeCheck();
            versionFoldout = EditorGUILayout.Foldout(versionFoldout, "【版本配置】");
            if(EditorGUI.EndChangeCheck())
            {
                foldoutChanged = true;
            }
            if(!versionFoldout) return;
            

            GUILayout.Space(3);
            EditorGUILayout.HelpBox("暂不支持资源版本配置",MessageType.Warning);
            using(new EditorGUI.DisabledScope(true))
            {
                //资源版本
                GUILayout.Space(3);
                GUILayout.BeginHorizontal();
                GUILayout.Label("资源版本", GUILayout.Width(100));
                string curResVersion = GUILayout.TextField(resVersion, GUILayout.Width(100));
                if (curResVersion != resVersion)
                {
                    resVersion = curResVersion;
                    GamePushAPI.SetResVersion(resVersion);
                }
                GUILayout.EndHorizontal();
            }
            

            //APP版本
            GUILayout.Space(3);
            GUILayout.BeginHorizontal();
            GUILayout.Label("APP版本", GUILayout.Width(100));
            string curBundleVersion = GUILayout.TextField(bundleVersion, GUILayout.Width(100));
            if (curBundleVersion != bundleVersion)
            {
                bundleVersion = curBundleVersion;
                PlayerSettings.bundleVersion = curBundleVersion;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(3);
            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();
        }
        #endregion

        #region 本地服务器配置GUI
        void DrawLocalServerGUI()
        {
            EditorGUI.BeginChangeCheck();
            localServerFoldout = EditorGUILayout.Foldout(localServerFoldout, "【开发环境】");
            if(EditorGUI.EndChangeCheck())
            {
                foldoutChanged = true;
            }
            if(!localServerFoldout) return;
            GUILayout.Space(3);
            
            GUILayout.BeginHorizontal();
            var curSelectedAsset = (AssetConfig)EditorGUILayout.EnumPopup("资源读取 : ", localAssetConfig, GUILayout.Width(300));
            bool typeChanged = curSelectedAsset != localAssetConfig;
            if (typeChanged)
            {
                GamePushAPI.SetAssetConfig(curSelectedAsset);
                localAssetConfig = curSelectedAsset;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(3);

            GUILayout.BeginHorizontal();

            var curSelectedNet = (NetConfig)EditorGUILayout.EnumPopup( new GUIContent("网络服务 : "), 
            localNetConfig,
            config=>(NetConfig)config != NetConfig.Online,
            true,
            GUILayout.Width(300));
            typeChanged = curSelectedNet != localNetConfig;
            if (typeChanged)
            {
                GamePushAPI.SetNetConfig(curSelectedNet);
                localNetConfig = curSelectedNet;
            }
            
            
            GUILayout.EndHorizontal();
            
        }
        #endregion

        #region AB相关操作GUI
        void DrawAssetBundlesGUIAsync()
        {
            EditorGUI.BeginChangeCheck();
            abFoldout = EditorGUILayout.Foldout(abFoldout, "【打包&发布】");
            if(EditorGUI.EndChangeCheck())
            {
                foldoutChanged = true;
            }
            if(!abFoldout) return;

            GUILayout.Space(3);

            GUILayout.BeginHorizontal();


            if (GUILayout.Button("打包AB",GUILayout.Width(100)))
            {
                log = "开始打包 请勿关闭窗口....";
                EditorApplication.delayCall += BuildAB;
            }
            

            if (GUILayout.Button("发布AB"))
            {
                EditorApplication.delayCall += PushAB;
            }
            using(new EditorGUI.DisabledScope(true))
            {
                if (GUILayout.Button("外发AB"))
                {
                }
            }
            
            if (GUILayout.Button("生成代码"))
            {
                EditorApplication.delayCall += YourNamespace.EditorTool.XluaBuild.Regenerated;
            }
            
            GUILayout.EndHorizontal();
            DrawLog();
        }

        private void OnLog(string obj)
        {
            //throw new NotImplementedException();
        }
        #endregion


        #region 打包相关GUI

        void DrawBuildPlayerGUI()
        {
                
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Build " + buildTarget.ToString(),  GUILayout.Width(200),GUILayout.Height(50)))
            {
                EditorApplication.delayCall += BuildProject.BuildDebug;
            }
            GUILayout.EndHorizontal();
        }
        #endregion
        private void DrawLog()
        {
            GUILayout.Space(3);
            GUILayout.Space(3);
            if(GUILayout.Button("清理日志",GUILayout.Width(100)))
            {
                log = "";
            }
            GUILayout.Space(3);
            GUILayout.TextArea(log,GUILayout.MinHeight(90));
        }


        private async void BuildAB()
        {

            int len = AutoAssetBundle.BuildAll();
            log = string.Format("总计生成{0}个文件，本次热更新文件如下...",len);
            var diff = await GamePushAPI.Update(OnLog);
            
            long totalSize = 0;
            foreach(var item in diff)
            {
                log = log + "\n" + string.Format("{0}, {1}kb" ,item.Key,item.Value.size/1024.0f);
                totalSize = totalSize + item.Value.size;
            }
            log = log + "\n" + string.Format("热更:{0}个文件, 大小{1}KB" ,diff.Count,totalSize/1024.0f);
        }
        private void PushAB()
        {
            if (GamePushAPI.UploadAB(out string uploadLog))
                log = log + "\n" + uploadLog;
            else
                log = log + "\n" + uploadLog;
        }
        void GUISeparator()
        {
            GUILayout.Space(4);
            if(EditorGUIUtility.isProSkin)
            {
                GUILine(new Color(.15f, .15f, .15f), 1);
                GUILine(new Color(.4f, .4f, .4f), 1);
            }
            else
            {
                GUILine(new Color(.3f, .3f, .3f), 1);
                GUILine(new Color(.9f, .9f, .9f), 1);
            }
            GUILayout.Space(4);
        }
        static public void GUILine(Color color, float height = 2f)
        {
            Rect position = GUILayoutUtility.GetRect(0f, float.MaxValue, height, height, LineStyle);

            if(Event.current.type == EventType.Repaint)
            {
                Color orgColor = GUI.color;
                GUI.color = orgColor * color;
                LineStyle.Draw(position, false, false, false, false);
                GUI.color = orgColor;
            }
        }
        static public GUIStyle _LineStyle;
        static public GUIStyle LineStyle
        {
            get
            {
                if(_LineStyle == null)
                {
                    _LineStyle = new GUIStyle();
                    _LineStyle.normal.background = EditorGUIUtility.whiteTexture;
                    _LineStyle.stretchWidth = true;
                }

                return _LineStyle;
            }
        }

    }
}

