/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;

namespace YourNamespace
{
    public class CSBehaviour : MonoBehaviour
    {

        internal static float lastGCTime = 0;
        internal const float GCInterval = 1;//1 second 

        private Action<LuaTable> luaStart;
        private Action<LuaTable> luaUpdate;
        private Action<LuaTable> luaOnDestroy;
        private Action<LuaTable> luaClick;

        private LuaTable scriptEnv;
        public static void AddComponent(GameObject go, LuaTable table)
        {
            
            var behaviour = go.AddComponent<CSBehaviour>();
            behaviour.Init(table);

        }

        public void Init(LuaTable table)
        {
            // scriptEnv = XluaManager.Ins.luaEnv.NewTable();

            // // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
            // LuaTable meta = XluaManager.Ins.luaEnv.NewTable();
            // meta.Set("__index", XluaManager.Ins.luaEnv.Global);
            // scriptEnv.SetMetaTable(meta);
            // meta.Dispose();
            
            scriptEnv = table;
            scriptEnv.Set("self", this);
            Action<LuaTable,GameObject> luaAwake = scriptEnv.Get<Action<LuaTable,GameObject>>("awake");
            scriptEnv.Get("start", out luaStart);
            scriptEnv.Get("update", out luaUpdate);
            scriptEnv.Get("ondestroy", out luaOnDestroy);
            scriptEnv.Get("onClick", out luaClick);

            if (luaAwake != null)
            {
                luaAwake(table,this.gameObject);
            }
        }

        // Use this for initialization
        void Start()
        {
            if (luaStart != null)
            {
                
                luaStart(scriptEnv);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (luaUpdate != null)
            {
                luaUpdate(scriptEnv);
            }
            if (Time.time - CSBehaviour.lastGCTime > GCInterval)
            {
                XluaManager.Ins.luaEnv.Tick();
                CSBehaviour.lastGCTime = Time.time;
            }
        }
        public void PointClick()
        {
            luaClick?.Invoke(scriptEnv);
        }

        void OnDestroy()
        {
            if (luaOnDestroy != null)
            {
                luaOnDestroy(scriptEnv);
            }
            luaOnDestroy = null;
            luaUpdate = null;
            luaStart = null;
            luaClick = null;
            if(scriptEnv!=null)scriptEnv.Dispose();
        }
    }
}