
using System;

namespace YourNamespace
{
    /// <summary>
    /// app 常量设置
    /// </summary>
    public class AppConst
    {
        public static NetConfig netConfig;
        public static AssetConfig assetConfig;

        private AppConst() { }
        /// <summary>
        /// 帧率
        /// </summary>
        public static int FrameRate => 30;
        public static int LowFrameRate => 20;

        private static AppVersion _version = AppVersion.None;
        /// <summary>
        /// APP版本
        /// </summary>
        public static AppVersion Version
        {
            get
            {
                if (_version == AppVersion.None)
                {
#if UNITY_EDITOR
                    _version = AppVersion.Alpha;
#else
                    _version = AppVersion.Production;//默认环境
#endif
                }
                return _version;
            }

        }

        public static void Init()
        {

#if UNITY_EDITOR
            //禁止手动切换到生产环境 防止干扰生产服务器数据
            // string forceNetConfig = LocalStorageMgr.Ins.Get(StorageKey.NetConfigEditor);
            // if (forceNetConfig != LocalStorageMgr.empty)
            //     netConfig = (NetConfig)Enum.Parse(typeof(NetConfig), forceNetConfig, true);
            // else
            //     netConfig = NetConfig.Test;
            netConfig = NetConfig.Test;
#else
            netConfig = Version == AppVersion.Production ? NetConfig.Online:NetConfig.Test;
#endif


#if UNITY_EDITOR
            string forceAssetConfig = LocalStorageMgr.Ins.Get(StorageKey.AssetConfigEditor);
            if (forceAssetConfig != LocalStorageMgr.empty)
                assetConfig = (AssetConfig)Enum.Parse(typeof(AssetConfig), forceAssetConfig, true);
            else
                assetConfig = AssetConfig.Res;
#else
            
            assetConfig = AssetConfig.OnlineAB;
            
#endif
        }

    }
    public enum AppVersion
    {
        None,
        Alpha,
        Production,
        Beta

    }

    /// <summary>
    /// 网络环境
    /// </summary>
    public enum NetConfig
    {
        //内网测试
        Test,
        //外网
        Online
    }
    /// <summary>
    /// 资源配置
    /// </summary>
    public enum AssetConfig
    {
        //编辑器开发 使用本地Resource
        Res,
        //编辑器开发 使用本地AssetBundle
        EditorAB,
        //使用在线AssetBundle
        OnlineAB
    }
}
