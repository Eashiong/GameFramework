using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;

namespace YourNamespace.Tool
{


    public class ABInfoWin : EditorWindow
    {
        private static Dictionary<string, AssetBundleDownloadInfo> diff;
        public static void OpenWindow()
        {
            ABInfoWin win = EditorWindow.GetWindow(typeof(ABInfoWin), false, "游戏管理", false) as ABInfoWin;
            //win.ShowNotification(GUILayout.Width);
        }

        private async void OnEnable() 
        {
            AutoAssetBundle.BuildAll();
            diff = new Dictionary<string, AssetBundleDownloadInfo>();
            diff = await GamePushAPI.Update(OnLog);
        }

        private void OnGUI() 
        {
            string s = "";
            long totalSize = 0;
            foreach(var item in diff)
            {
                s = s + "\n" + string.Format("{0}, {1}kb" ,item.Key,item.Value.size/1024.0f);
                totalSize = totalSize + item.Value.size;
            }
            s = s + "\n" + string.Format("总计:{0}个文件, {1}KB" ,diff.Count,totalSize/1024.0f);
            GUILayout.TextArea(s);
        }
        private void OnLog(string log)
        {

        }

    }
}
