using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using UnityEngine;
using System.Threading.Tasks;

namespace YourNamespace
{

    public enum UpdateType
    {
        None,
        Update,         // 不需要更新
        FouceUpdate,       // 强制更新

    }

    public class AssetUpdate
    {
        public Action<float, string> process;


        private string localABInfolistPath;
        private string serverInfolistPath;
        
        public async Task Run()
        {

            /*
                1，检测版本号
                2，执行跟新策略
            */
                
            localABInfolistPath = Path.Combine(URIHelper.LocalABPath,"version.json");
            serverInfolistPath = Path.Combine(URIHelper.RemoteABPathEditor,"version.json");

            UpdateType updateType = await CheckAppUpdate();
            if(updateType == UpdateType.Update)
            {
                await Update();
            }
        }

        private async Task<UpdateType> CheckAppUpdate()
        {
            process?.Invoke(0.0f, "正在检查APP版本");
            await Task.Delay(100);
            process?.Invoke(1, "正在检查APP版本");
            UpdateType updateType = UpdateType.Update;
            return updateType;

        }

        private async Task Update()
        {
            string localText = await DownloadAsync.DownLoadText(localABInfolistPath);
            string serverText = await DownloadAsync.DownLoadText(serverInfolistPath);
            if(string.IsNullOrEmpty(serverText))
            {
                process?.Invoke(0.0f, "服务器错误,请退出重试");
                while(true) await Task.Delay(1000);
            }
            Dictionary<string, AssetBundleDownloadInfo> downloadList = ContrastUpdate(localText, serverText);

            Log.Blue("asset", "需要更新的资源数量 ======== " + downloadList.Count);

            if (downloadList.Count == 0)
                return;

            process?.Invoke(0.0f, "资源下载中...");
            await DownLoadFromList(downloadList);
            Log.Blue("asset", "资源已经同步完成，开始同步清单文件");
            System.IO.File.WriteAllText(localABInfolistPath,serverText);
        }
        //差异跟新
        private  Dictionary<string, AssetBundleDownloadInfo> ContrastUpdate(string localText, string serverText)
        {
            Log.White("asset", "开始对比文件差异:" + serverText);
            AssetBundleDownloadInfoList localInfolist = string.IsNullOrEmpty(localText)? 
                new AssetBundleDownloadInfoList(): JsonUtility.FromJson<AssetBundleDownloadInfoList>(localText);

            AssetBundleDownloadInfoList serverInfolist = JsonUtility.FromJson<AssetBundleDownloadInfoList>(serverText);

            Log.White("asset", "开始查找需要跟新的资源");
            return  FindDifference(localInfolist, serverInfolist);
        }

        //对比2分清单文件的差异 找出下载内容
        private Dictionary<string, AssetBundleDownloadInfo> FindDifference(AssetBundleDownloadInfoList local, AssetBundleDownloadInfoList server)
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


        private async Task DownLoadFromList(Dictionary<string, AssetBundleDownloadInfo> downloadList)
        {
            float totalSize = 0;
            float curSize = 0;
            foreach (var item in downloadList)
                totalSize+= item.Value.size;
            totalSize =  Mathf.Max(0.1f, ((int)( 10 * (totalSize/1024/1024) )) *0.1f);
            foreach (var item in downloadList)
            {
                string assetPath = item.Key;
                string rometeUrl;
                if(AppConst.assetConfig == AssetConfig.OnlineAB)
                {
                    rometeUrl = Path.Combine(URIHelper.RemoteABPathEditor,item.Key + "?md5=" + item.Value.md5);
                }
                else
                {
                    rometeUrl = Path.Combine(URIHelper.RemoteABPathEditor,item.Key);
                }
                
                string localPath = Path.Combine(URIHelper.LocalABPath, item.Key);
                Log.White("asset",string.Format("正在下载{0}({1} bytes)", item.Key,item.Value.size));

                //重试
                string result = null;
                while(string.IsNullOrEmpty(result))
                {
                    result = await DownloadAsync.DownloadFile(rometeUrl,localPath);
                    if(string.IsNullOrEmpty(result))
                        await Task.Delay(300);
                }
  
                
                curSize += item.Value.size;
                curSize = ((int)( 10 * (curSize/1024/1024) )) *0.1f;
                float progressValue = Mathf.Lerp(0,100.0f,curSize / totalSize);
                process?.Invoke(progressValue, string.Format("正在更新资源:{0}M/{1}M",curSize,totalSize));
                Log.White("asset",string.Format("更新进度{0}%", progressValue));

            }
        }
    }
    

}
