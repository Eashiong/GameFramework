
//时间戳换算


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
namespace YourNamespace
{

    /// <summary>
    /// 时间戳
    /// </summary>
    public static class TimeHelp
    {

        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long Convert2Unix(System.DateTime time)
        {
            System.DateTime startTime = TimeZoneInfo.ConvertTime(new System.DateTime(1970, 1, 1, 8, 0, 0, 0), TimeZoneInfo.Local);
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位
            return t;
        }
        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name=”timeStamp”></param>
        /// <returns></returns>
        public static DateTime Convert2DateTime(string timeStamp)
        {
            DateTime dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 8, 0, 0, 0), TimeZoneInfo.Local);
            long lTime;
            if (timeStamp.Length.Equals(10))//判断是10位
                lTime = long.Parse(timeStamp + "0000000");
            else
                lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        //和现在的时间进行比较
        public static TimeSpan CompareNow(string timeStamp)
        {
            return System.DateTime.Now - TimeHelp.Convert2DateTime(timeStamp);
        }
        //和时间进行比较
        public static TimeSpan Compare(string timeStamp1,string timeStamp2)
        {
            return TimeHelp.Convert2DateTime(timeStamp1) - TimeHelp.Convert2DateTime(timeStamp2);
        }
        

    }


}