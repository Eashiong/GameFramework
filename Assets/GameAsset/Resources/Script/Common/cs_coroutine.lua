-- Tencent is pleased to support the open source community by making xLua available.
-- Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
-- Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
-- http://opensource.org/licenses/MIT
-- Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

local util = require "Common.util"

local gameobject = CS.UnityEngine.GameObject("Coroutine_Runner")
CS.UnityEngine.Object.DontDestroyOnLoad(gameobject)
local cs_coroutine_runner = gameobject:AddComponent(typeof(CS.YourNamespace.Coroutine_Runner))

cs_coroutine = {}
function cs_coroutine.start(...)
    return cs_coroutine_runner:StartCoroutine(util.cs_generator(...))
end
function cs_coroutine.stop(coroutine)
    if coroutine~= nil and cs_coroutine_runner~=nil then
        cs_coroutine_runner:StopCoroutine(coroutine)
        coroutine = nil
    end
end
function cs_coroutine.dispose()
    cs_coroutine_runner = nil
    GameObject.Destroy(gameobject)
end

function cs_coroutine.WaitForSeconds(seconds)
	coroutine.yield(CS.UnityEngine.WaitForSeconds(seconds))
end

function cs_coroutine.WaitForUpdate()
	coroutine.yield(CS.YourNamespace.WaitForUpdate())
end
