/*
 * @Description: 全局管理器
 */


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YourNamespace
{
    /// <summary>
    /// 入口 全局管理期
    /// </summary>
    public class Manager : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
        private void Start()
        {
            //LOG
            Log.InitTag();
            if (AppConst.Version == AppVersion.Production)
                Log.OnlyProduction();
                
            AppConst.Init();

            System.AppDomain.CurrentDomain.UnhandledException += (sender, e) => Log.Red("comm", e.ToString());
            Application.lowMemory += () => Log.Red("comm", "内存峰值警告!");

            //根据设备配置自动设置质量
            DevicePerformanceUtil.ModifySettingsBasedOnPerformance();

            XluaManager.Ins.StartUp();




        }
    }
}
