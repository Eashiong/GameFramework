//获取设备性能评级
using UnityEngine;
namespace YourNamespace
{
    public static class DevicePerformanceUtil
    {
        /// <summary>
        /// 获取设备性能评级
        /// </summary>
        /// <returns>性能评级</returns>        
        public static DevicePerformanceLevel GetDevicePerformanceLevel()
        {
            
            Debug.Log("显卡标识符  " + SystemInfo.graphicsDeviceVendorID);
            if (SystemInfo.graphicsDeviceVendorID == 32902)
            {
                //集显
                return DevicePerformanceLevel.Low;
            }
            else //NVIDIA系列显卡（N卡）和AMD系列显卡
            {
                Debug.Log("CPU核心数:" + SystemInfo.processorCount);
                //根据目前硬件配置三个平台设置了不一样的评判标准（仅个人意见）
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                if (SystemInfo.processorCount <= 2)
#elif UNITY_STANDALONE_OSX || UNITY_IPHONE
            if (SystemInfo.processorCount < 2)
#elif UNITY_ANDROID
            if (SystemInfo.processorCount <= 4)
#endif
                {
                    //CPU核心数<=2判定为低端
                    return DevicePerformanceLevel.Low;
                }
                else
                {
                    //显存
                    int graphicsMemorySize = SystemInfo.graphicsMemorySize;
                    Debug.Log("显存:" + graphicsMemorySize);
                    //内存
                    int systemMemorySize = SystemInfo.systemMemorySize;
                    Debug.Log("内存:" + systemMemorySize);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                    if (graphicsMemorySize >= 4000 && systemMemorySize >= 8000)
                        return DevicePerformanceLevel.High;
                    else if (graphicsMemorySize >= 2000 && systemMemorySize >= 4000)
                        return DevicePerformanceLevel.Mid;
                    else
                        return DevicePerformanceLevel.Low;
#elif UNITY_STANDALONE_OSX || UNITY_IPHONE
            if (graphicsMemorySize >= 4000 && systemMemorySize >= 8000)
                    return DevicePerformanceLevel.High;
                else if (graphicsMemorySize >= 2000 && systemMemorySize >= 4000)
                    return DevicePerformanceLevel.Mid;
                else
                    return DevicePerformanceLevel.Low;
#elif UNITY_ANDROID
            if (graphicsMemorySize >= 6000 && systemMemorySize >= 8000)
                    return DevicePerformanceLevel.High;
                else if (graphicsMemorySize >= 2000 && systemMemorySize >= 4000)
                    return DevicePerformanceLevel.Mid;
                else
                    return DevicePerformanceLevel.Low;
#endif
                }
            }
        }

        /// <summary>
        /// 根据手机性能修改项目设置
        /// </summary>
        public static void ModifySettingsBasedOnPerformance()
        {
            DevicePerformanceLevel level = GetDevicePerformanceLevel();
            Debug.Log("设备质量:" + level);
            switch (level)
            {
                case DevicePerformanceLevel.Low:
                    //设置帧率
                    SetQualitySettings(QualityLevel.Low);
                    //设置Quality级别
                    QualitySettings.SetQualityLevel((int)QualityLevel.Low + 1, true);
                    break;
                case DevicePerformanceLevel.Mid:
                case DevicePerformanceLevel.High:
                    //设置帧率
                    SetQualitySettings(QualityLevel.Mid);
                    //设置Quality级别
                    QualitySettings.SetQualityLevel((int)QualityLevel.Mid + 1, true);
                    break;
            }
        }

        /// <summary>
        /// 根据自身需要调整各级别需要修改的设置
        /// </summary>
        /// <param name="qualityLevel">质量等级</param>
        public static void SetQualitySettings(QualityLevel qualityLevel)
        {
            switch (qualityLevel)
            {
                case QualityLevel.Low:
                    UnityEngine.Time.fixedDeltaTime = 0.05f;
                    UnityEngine.Time.maximumDeltaTime = 0.45f;
                    Application.targetFrameRate = AppConst.LowFrameRate;
                    break;
                case QualityLevel.Mid:
                    UnityEngine.Time.fixedDeltaTime = 0.03f;
                    UnityEngine.Time.maximumDeltaTime = 0.27f;
                    Application.targetFrameRate = AppConst.FrameRate;
                    break;
            }
        }
    }
    public enum DevicePerformanceLevel
    {
        Low,
        Mid,
        High
    }

    public enum QualityLevel
    {
        Low,
        Mid,
        // High, //可忽略。项目中只用两种质量设置
    }
}
