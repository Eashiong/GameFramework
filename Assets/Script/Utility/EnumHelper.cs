using System;
using System.ComponentModel;

namespace YourNamespace
{
    public static class EnumHelper
    {
        /// <summary>
        /// 获得枚举的Description
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <param name="nameInstead">当枚举值没有定义DescriptionAttribute，是否使用枚举名代替，默认是使用</param>
        /// <returns>枚举的Description</returns>
        public static string GetDescription(this Enum value, bool nameInstead = true)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name == null)
            {
                return null;
            }

            System.Reflection.FieldInfo field = type.GetField(name);
            DescriptionAttribute attribute = System.Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        
            if (attribute == null && nameInstead == true)
            {
                return name;
            }
            return attribute?.Description;
        }
        /// <summary>
        /// 根据Description获取枚举值
        /// </summary>
        /// <param name="description">枚举说明符</param>
        /// <returns>枚举值</returns>
        public static T GetValueByDescription<T>(string description) where T:Enum
        {
            foreach(var item in Enum.GetValues(typeof(T)))
            {   
                string str = GetDescription(item as Enum);
                if(str == description)
                {
                    return (T)item;
                }
            }
            Log.Red("comm",typeof(T).Name + "未定义说明符:" + description);
            return default;
        }
    }
}
