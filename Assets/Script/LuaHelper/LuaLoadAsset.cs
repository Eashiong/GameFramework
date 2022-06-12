using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace YourNamespace
{

    /// <summary>
    /// lua侧使用 ,c#侧不使用该类 使用AssetMgr替代
    /// </summary>
    public static class LuaLoadAsset
    {
        public static GameObject LoadPrefab(this AssetMgr mgr, string name)
        {
            return mgr.Load<GameObject>(name);
        }
        public static string LoadText(this AssetMgr mgr, string name)
        {
            return mgr.Load<TextAsset>(name).text;
        }
        public static RuntimeAnimatorController LoadAni(this AssetMgr mgr, string name)
        {
            return mgr.Load<RuntimeAnimatorController>(name);
        }
        public static Sprite LoadSprite(this AssetMgr mgr, string name)
        {
            return mgr.Load<Sprite>(name);
        }
    }
}
