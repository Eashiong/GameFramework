/*
 * @Description: 字典扩展
 */


using System.Collections.Generic;
namespace YourNamespace
{
    /// <summary>
    /// 对字典进行扩展 简写代买
    /// </summary>
    public static class DictionaryHelper
    {
        /// <summary>
        /// 修改值 如果没有键则添加
        /// </summary>
        public static void AddOrSet<Tkey, TValue>(this Dictionary<Tkey, TValue> dic, Tkey key, TValue value)
        {
            if (dic.ContainsKey(key))
                dic[key] = value;
            else
                dic.Add(key, value);
        }
        /// <summary>
        /// 获取值 如果没有键返回空
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        public static TValue SafeGet<Tkey, TValue>(this Dictionary<Tkey, TValue> dic, Tkey key) where TValue : class
        {
            TValue value;
            if (dic.TryGetValue(key, out value))
                return value;
            else return null;
        }
    }
}