
using UnityEngine;

namespace YourNamespace
{
    /// <summary>
    /// 绑定每个从AB加载出来的prefab
    /// </summary>
    public class AssetInfo : MonoBehaviour
    {
        private string assetkey;
        public void Init(string assetkey) => this.assetkey = assetkey;
        private void OnDestroy()
        {
            if(!string.IsNullOrEmpty(assetkey))
                AssetMgr.Ins.Unload(assetkey);
        }
    }
}