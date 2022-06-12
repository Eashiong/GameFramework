using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
namespace YourNamespace
{
    public class XluaManager : MonoSingleton<XluaManager>
    {
        public LuaEnv luaEnv { get;private set;}  = null;

        protected override void OnInit()
        {
            base.OnInit();
            InitLuaEnv();
        }

        public void InitLuaEnv()
        {
            luaEnv = new LuaEnv();
            if (luaEnv != null)
            {
                this.luaEnv.AddLoader(this.CustomLoader);
                this.DoString("require 'Main.LuaMain'");
// #if HOTFIX_ENABLE
//                 this.DoString("require 'Main.CommHotfix'");
// #endif
                
            }
            else
            {
                Debug.LogError("InitLuaEnv null");
            }
        }
        private byte[] CustomLoader(ref string filepath)
        {
    
            return AssetMgr.Ins.LoadScript(ref filepath);
        }

        public void DoString(string scriptName,string chunkName = "chunk", LuaTable env = null)
        {
            if (luaEnv != null)
            {
                try
                {
                    luaEnv.DoString(scriptName,chunkName,env);
                }
                catch (System.Exception ex)
                {
                    string msg = string.Format("xLua exception : {0}\n {1}", ex.Message, ex.StackTrace);
                    Debug.LogError(msg);
                }
            }
        }
        private void OnDestroy() {
            QuitLua();
        }
        public void QuitLua()
        {
            
                if (luaEnv != null)
                {
                    luaEnv.Dispose();
                    luaEnv = null;
                }
                 
            
            
        }
    }
}