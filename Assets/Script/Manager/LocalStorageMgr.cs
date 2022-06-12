using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YourNamespace
{
    /// <summary>
    /// 本地存储关键字
    /// </summary>
    public enum StorageKey
    {


        NetConfigEditor,
        AssetConfigEditor,
        FirstRunApp,//是否第一次运行APP
    }

    /// <summary>
    /// 本地数据存储管理
    /// </summary>
    public class LocalStorageMgr:Singleton<LocalStorageMgr>
    {

        public static string empty;
        //创建存储key
        protected override void OnInit()
        {
            base.OnInit();
            empty = string.Empty;
            InitLocalKey();
        }

        // 保存
        public void Save(StorageKey storageKey, string value,bool autoSave = true)
        {
            Log.Blue("Local", string.Format("准备保存，key:{0},value:{1}", storageKey.ToString(), value));
            string key = GetKey(storageKey);
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetString(key, value);
                Log.White("Local", string.Format("保存成功，key:{0},value:{1}", key, value));
            }
            else
            {
                Log.Red("Local", string.Format("保存失败，key:{0},value:{1}", key, value));
            }
            if(autoSave)
                Apply();

        }
        public string Get(StorageKey storageKey)
        {
            string key = GetKey(storageKey);
            string value = empty;
            if (PlayerPrefs.HasKey(key))
            {
                value = PlayerPrefs.GetString(key);
                Log.White("Local", string.Format("取值成功，key:{0},value:{1}", key, value));
            }
            else
            {
                Log.Red("Local", string.Format("取值失败，key:{0}", key));
            }
            return value;
        }

        // 存储一个bool
        public void SaveBool(StorageKey storageKey, bool value,bool autoSave = false)
        {
            Log.Blue("Local", string.Format("准备保存，key:{0},value:{1}", storageKey.ToString(), value));
            string key = GetKey(storageKey);
            if (PlayerPrefs.HasKey(key))
            {
                string _value = value ? "1" : empty;
                PlayerPrefs.SetString(key, _value);
                Log.White("Local", string.Format("保存成功，key:{0},value:{1}", key, value));
            }
            else
            {
                Log.Red("Local", string.Format("保存失败，key:{0},value:{1}", key, value));
            }
            if(autoSave)
                Apply();
        }
        // 查询一个bool
        public bool GetBool(StorageKey storageKey)
        {
            string key = GetKey(storageKey);
            bool value = false;
            if (PlayerPrefs.HasKey(key))
            {
                string _value = PlayerPrefs.GetString(key);
                value = _value != empty;
                Log.White("Local", string.Format("取值成功，key:{0},value:{1}", key, value));
            }
            else
            {
                Log.Red("Local", string.Format("取值失败，key:{0}", key));
            }
            return value;

        }

        public void Apply()
        {
            PlayerPrefs.Save();
        }

        public void Delete(StorageKey storageKey,bool autoSave = true)
        {
            Log.White("Local", string.Format("准备删除，key:{0}", storageKey.ToString()));
            string key = GetKey(storageKey);
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetString(key, empty);
                Log.White("Local", string.Format("删除成功，key:{0},value:{1}", key, empty));
            }
            else
            {
                Log.Red("Local", string.Format("删除失败，key:{0},value:{1}", key, empty));
            }
            if(autoSave)
                Apply();

        }

        public void ClearAll(bool autoSave = true)
        {
            foreach (var name in Enum.GetNames(typeof(StorageKey)))
            {
                if (PlayerPrefs.HasKey(name))
                {
                    PlayerPrefs.SetString(name,empty);              
                }
            }
            if(autoSave)
                Apply();
            Log.Blue("Local", "删除本地数据成功");
        }

        private string GetKey(StorageKey storageKey)
        {
            return storageKey.ToString();

        }
        private void InitLocalKey()
        {
            foreach (var name in Enum.GetNames(typeof(StorageKey)))
            {
                if (!PlayerPrefs.HasKey(name))
                {
                    PlayerPrefs.SetString(name, empty);
                }
            }

            PlayerPrefs.Save();
        }
    }
}
