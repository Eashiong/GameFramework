using System;
using System.IO;
using UnityEngine;

namespace YourNamespace
{

    /// <summary>
    /// 构建Uri 用于下载
    /// </summary>
    public static class URIHelper
    {
        public static string baseUri;
        /// <summary>
        /// 远端资源包路径
        /// </summary>
        /// <value></value>
        public static string RemoteABPathEditor
        {
            get
            {
                if (AppConst.assetConfig == AssetConfig.EditorAB)
                    //return Path.Combine(System.Environment.CurrentDirectory,"Res/iOS");
#if UNITY_IOS
                    return Path.Combine(Application.streamingAssetsPath, "Games/iOS");
#else
                    return Path.Combine(Application.streamingAssetsPath, $"Games/Android");
#endif
                else if (AppConst.assetConfig == AssetConfig.OnlineAB)
                    return baseUri;
                else
                    return "";
            }
        }
        /// <summary>
        /// 本地资源包路径
        /// </summary>
        /// <value></value>
        public static string LocalABPath
        {
            get
            {
                string path = Path.Combine(Application.persistentDataPath, "Games/");
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
                return path;
            }
        }
        public static Uri Builder(string downloadPath)
        {
            System.Uri uri = null;
            try
            {
                uri = new System.Uri(downloadPath);
            }
            catch (System.Exception e)
            {
                Log.Red("comm", e.ToString());
                Log.Red("comm", "无法解析地址:" + downloadPath);
                uri = null;
            }
            return uri;
        }

    }
}