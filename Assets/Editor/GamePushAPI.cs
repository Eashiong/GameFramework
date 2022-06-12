//项目管理工具

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Linq;
namespace YourNamespace.Tool
{
    public static class GamePushAPI
    {
        private static System.Action<string> msg;
        public static string GetAPPVersion()
        {
            return "1.0";
        }
        public static string GetResVersion()
        {
            return "1.0";
        }
        public static bool SetResVersion(string version)
        {
            return true;
        }

        public static async Task<Dictionary<string, AssetBundleDownloadInfo>> Update(System.Action<string> _msg) 
        {
            msg = _msg;

            string localABInfolistPath = Application.streamingAssetsPath + "/Games/"  + BuildTarget.Android.ToString() + "/version.json";
            string serverInfolistPath = "https://........com/a/games/"  + "version.json";

            string localText = await DownloadAsync.DownLoadText(localABInfolistPath);
            string serverText = await DownloadAsync.DownLoadText(serverInfolistPath);

            return  ContrastUpdate(localText, serverText);
        }
        private static  Dictionary<string, AssetBundleDownloadInfo> ContrastUpdate(string localText, string serverText)
        {
            msg("开始对比文件差异:" + serverText);
            AssetBundleDownloadInfoList localInfolist = string.IsNullOrEmpty(localText)? 
                new AssetBundleDownloadInfoList(): JsonUtility.FromJson<AssetBundleDownloadInfoList>(localText);

            AssetBundleDownloadInfoList serverInfolist = JsonUtility.FromJson<AssetBundleDownloadInfoList>(serverText);

            msg("开始查找需要跟新的资源");
            return  FindDifference(localInfolist, serverInfolist);
        }

        public static AssetConfig GetAssetConfig()
        {
            string forceAssetConfig = LocalStorageMgr.Ins.Get(StorageKey.AssetConfigEditor);
            if (forceAssetConfig != LocalStorageMgr.empty)
                return (AssetConfig)Enum.Parse(typeof(AssetConfig), forceAssetConfig, true);
            else
                return AssetConfig.Res;
        }
        public static void SetAssetConfig(AssetConfig config)
        {
            LocalStorageMgr.Ins.Save(StorageKey.AssetConfigEditor,config.ToString());
        }
    
        public static NetConfig GetNetConfig()
        {
            string forceNetConfig = LocalStorageMgr.Ins.Get(StorageKey.NetConfigEditor);
            if (forceNetConfig != LocalStorageMgr.empty)
                return (NetConfig)Enum.Parse(typeof(NetConfig), forceNetConfig, true);
            else
                return NetConfig.Test;
        }
        public static void SetNetConfig(NetConfig config)
        {
            LocalStorageMgr.Ins.Save(StorageKey.NetConfigEditor,config.ToString());
        }

        //对比2分清单文件的差异 找出下载内容
        private static Dictionary<string, AssetBundleDownloadInfo> FindDifference(AssetBundleDownloadInfoList local, AssetBundleDownloadInfoList server)
        {

            Dictionary<string, AssetBundleDownloadInfo> downloadList = new Dictionary<string, AssetBundleDownloadInfo>();
            //注意一定是根据服务器去查本地 而不能反过来
            foreach (var info_server in server.infos)
            {
                AssetBundleDownloadInfo sameAsset = null;
                foreach(var info_local in local.infos)
                {
                    //本地存在该资源 但是md5和服务器不一致 需要替换跟新
                    if(info_local.name == info_server.name)
                    {
                        sameAsset = info_local;
                        break;
                    }
                }
                //本地没有资源 或者本地资源需要跟新
                if(sameAsset == null ||  sameAsset.md5!= info_server.md5)
                {
                    downloadList.Add(info_server.name,info_server);
                }
            }
            return downloadList;
        }

        //上传AB包到内网测试 返回结果和日志信息
        public static bool UploadAB(out string log)
        {
            try
            {
                Debug.Log("上传android");
                string projectPath = Path.Combine("Assets/StreamingAssets/Games/");
                string from = Path.Combine(projectPath,BuildTarget.Android.ToString());
                string to = "/Users/ifi/Desktop/games";
                CopyABFile(from,to);
                Debug.Log("上传ios");
                from = Path.Combine(projectPath,BuildTarget.iOS.ToString());
                CopyABFile(from,to);
                log = "scuess";
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
                log = e.ToString();
                return false;
            }
            return true;
        }

        private static void CopyABFile(string source, string target)
        {
            try
            {
                if(!Directory.Exists(target))
                {
                    Directory.CreateDirectory(target);
                }
                var files = Directory.GetFiles(source).Where( s => s.EndsWith(".ab") || s.EndsWith(".manifest") || s.EndsWith(".json"));
                foreach(var s in files)
                {
                    string fileName = Path.GetFileName(s);
                    string targetFileName = Path.Combine(target, fileName);
                    File.Copy(s, targetFileName, overwrite: true);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("拷贝失败");
                Debug.LogError(e.ToString());
            }

        }
    }

}
