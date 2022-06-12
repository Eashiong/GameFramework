using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YourNamespace
{


    [System.Serializable]
    public class AssetBundleDownloadInfo
    {
        public string name;
        public long size;
        public string md5;
    }
    [System.Serializable]
    public class AssetBundleDownloadInfoList
    {
        public List<AssetBundleDownloadInfo> infos = new List<AssetBundleDownloadInfo>();
    }
}