using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace YourNamespace
{
    /// <summary>
    /// 资源加载器
    /// </summary>
    public abstract class AssetLoader
    {
        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="localFile">Resource下的资源名 例如"Simple/Simple"</param>
        /// <typeparam name="T">Object</typeparam>
        /// <returns></returns>
        public abstract T Load<T>(string localFile) where T : UnityEngine.Object;
        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="localFile">Resource下的资源名 例如"Simple/Simple"</param>
        public abstract void Unload(string localFile);
    }
    /// <summary>
    /// 资源加载管理
    /// </summary>
    public class AssetMgr : Singleton<AssetMgr>
    {
        private AssetLoader loader;
        protected override void OnInit()
        {
            base.OnInit();
            if (AppConst.assetConfig == AssetConfig.Res)
                loader = new ResAsset();
            else
                loader = new ABAsset();
        }
        /// <summary>
        /// 读取资源
        /// </summary>
        /// <param name="localFile">Resource下的资源名 例如"Simple/Simple"</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Load<T>(string localFile) where T : UnityEngine.Object
        {
            return loader.Load<T>(localFile);
        }
        /// <summary>
        /// 读取代码文件 禁止调用
        /// </summary>
        /// <returns></returns>
        public byte[] LoadScript(ref string filepath)
        {
            if (AppConst.assetConfig == AssetConfig.Res)
            {
                filepath = "Script/" + filepath.Replace(".", "/");
                return (loader as ResAsset).Load<TextAsset>(filepath).bytes;
            }
            else
            {
                string[] s = filepath.Split('.');
                filepath = s[s.Length - 1];
                return (loader as ABAsset).LoadDefault().ab.LoadAsset<TextAsset>(filepath).bytes;
            }

        }
        /// <summary>
        /// 自动卸载资源 禁止调用
        /// </summary>
        public void Unload(string localFile)
        {
            if (loader is ABAsset)
                (loader as ABAsset).Unload(localFile);
        }
    }
    /// <summary>
    /// resources资源
    /// </summary>
    public class ResAsset : AssetLoader
    {
        /// <summary>
        /// 读取资源
        /// </summary>
        /// <param name="localFile">Resource下的资源名 例如"Simple/Simple"</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override T Load<T>(string localFile)
        {
            if (string.IsNullOrEmpty(localFile))
            {
                Log.Red("asset", "资源路径不能为空");
                return default;
            }
            T t = Resources.Load<T>(localFile);
            if (t == null)
            {
                Log.Red("asset", "资源加载失败:" + localFile);
                return default;
            }
            if (typeof(T) == typeof(GameObject))
            {
                string name = t.name;
                t = GameObject.Instantiate(t);
                t.name = name;
            }
            return t;
        }

        public override void Unload(string localFile) { }
    }

    /// <summary>
    /// ab资源
    /// </summary>
    public class ABAsset : AssetLoader
    {
        private readonly Dictionary<string, ABInfo> abMap;
        //脚本AB包
        private ABInfo luaScript;
        //依赖信息
        private AssetBundleManifest manifest;


        public ABAsset()
        {
            abMap = new Dictionary<string, ABInfo>();

        }
        //读取先决条件资源包
        public ABInfo LoadDefault()
        {
            if (manifest == null)
            {
                string anName;
#if UNITY_IOS
                anName = "iOS.ab";
#else
                anName = "Android.ab";
#endif
                AssetBundle ab = AssetBundle.LoadFromFile(Path.Combine(URIHelper.LocalABPath, anName));
                manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
            if (luaScript == null)
                luaScript = CreateABInfo(URIHelper.LocalABPath, "script.ab");
            return luaScript;
        }

        public override T Load<T>(string localFile)
        {
            if (!GetABFileName(localFile, out string abName, out string assetName))
            {
                return default;
            }
            ABInfo abInfo = CreateABInfo(URIHelper.LocalABPath, abName);
            if (abInfo == null) return default;
            return abInfo.CreateAsset<T>(assetName);
        }
        private bool GetABFileName(string localFile, out string anName, out string assetName)
        {
            try
            {
                string[] s = localFile.Split('/');
                anName = s[0].ToLower() + ".ab";
                assetName = s[1];
                return true;
            }
            catch
            {
                anName = assetName = "";
                return false;
            }

        }

        private ABInfo CreateABInfo(string baseUri, string abName)
        {
            string uri = Path.Combine(baseUri, abName);
            ABInfo abInfo;
            if (abMap.ContainsKey(abName))
            {
                if (abMap[abName] == null)
                {
                    // AssetBundle ab = AssetBundle.LoadFromFile(localFile);
                    // if (ab == null)
                    // {
                    //     Log.Red("asset", "AB包读取失败:" + localFile);
                    //     return null;
                    // }
                    // abInfo = new ABInfo(ab, localFile);
                    // abMap[localFile] = abInfo;
                    Debug.LogError("AB value fail !!!");
                    return null;
                }
                else
                {
                    abInfo = abMap[abName];
                }
            }
            else
            {
                string[] dependencies = manifest.GetAllDependencies(abName);
                for (int i = 0; i < dependencies.Length; i++)
                {
                    CreateABInfo(URIHelper.LocalABPath, dependencies[i]);
                }
                AssetBundle ab = AssetBundle.LoadFromFile(uri);
                if (ab == null)
                {
                    Log.Red("asset", "AB包读取失败:" + uri);
                    return null;
                }
                abInfo = new ABInfo(ab, abName);
                abMap.Add(abName, abInfo);
            }
            return abInfo;
        }
        /// <summary>
        /// 自动卸载 prefab类型资源
        /// </summary>
        /// <param name="assetKey">资源标识</param>
        /// <param name="check">调用验证 仅T类型可调用该方法</param>
        /// <typeparam name="Check">调用验证 仅T类型可调用该方法</typeparam>
        public override void Unload(string assetKey)
        {
            if (!abMap.ContainsKey(assetKey))
            {
                Log.Red("asset", "重复卸载:" + assetKey);
                return;
            }

            ABInfo info = abMap[assetKey];
            if (info == null)
            {
                Log.Red("asset", "卸载资源出现异常 abInfo 空");
                return;
            }
            int count = info.RemoveRes();
            if (count <= 0)
            {
                abMap.Remove(assetKey);
            }
        }
        /// <summary>
        /// 手动卸载 非prefab类型资源
        /// <para> 例如动画 贴图 材质等 </para>
        /// <para> 一般情况这些资源都是依赖在prefab上 不需要单独单独管理，我们项目特殊是作为单独资产使用的,必须手动调用 </para>
        /// </summary>
        /// <param name="assetKey">资源标识</param>
        public void UnloadAsset(string assetKey)
        {
            if (!abMap.ContainsKey(assetKey))
            {
                Log.Red("asset", "重复卸载:" + assetKey);
                return;
            }

            ABInfo info = abMap[assetKey];
            if (info == null)
            {
                Log.Red("asset", "卸载资源出现异常 abInfo 空");
                return;
            }
            if (info.asset != null && info.asset is GameObject)
            {
                Log.Red("asset", "无法手动卸载prefab类型的资源");
                return;
            }
            info.RemoveRes();
            abMap.Remove(assetKey);
        }

        public void ClearAll()
        {
            Log.White("asset", "ClearAll");
            foreach (var item in abMap)
                item.Value?.RemoveRes(forceRemove: true);
            abMap.Clear();
        }

    }
    /// <summary>
    /// AB包内容
    /// </summary>
    public class ABInfo
    {
        //压缩包
        public AssetBundle ab { get; private set; }
        //包内资源
        public Object asset { get; private set; }
        //资源实例化数目
        private int resCount;
        private string assetkey;

        public ABInfo(AssetBundle ab, string assetKey)
        {
            this.assetkey = assetKey;
            this.ab = ab;

            this.resCount = 0;
        }
        public T CreateAsset<T>(string assetName) where T : Object
        {
            //if(this.asset == null)
            //{
            this.asset = ab.LoadAsset<T>(assetName);

            //}
            if (typeof(T) == typeof(GameObject))
            {
                resCount++;
                T clone = Object.Instantiate<T>(this.asset as T);
                clone.name = asset.name;
                (clone as GameObject).AddComponent<AssetInfo>().Init(assetkey);
                Log.White("asset", "加载:" + assetkey);
                return clone;
            }
            Log.White("asset", "加载:" + assetkey);
            return this.asset as T;

        }
        //移除引用
        public int RemoveRes(bool forceRemove = false)
        {
            if (--resCount <= 0 || forceRemove)
            {
                Log.Blue("asset", "已卸载:" + assetkey);
                ab.Unload(true);
            }
            return resCount;

        }
    }

}
