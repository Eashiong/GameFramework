--[[
FilePath: /xlua_v2.1.15/Assets/Resources/Script/Common/CommHotfix.lua
Description: 
--]]

-- 所有需要热修复的脚步

CommHotfix = {}
local this = CommHotfix

-- 需要被加载的热修复模块
-- 自己的写的修复lua脚本都要加到这里
this.modules =
{
	-- 示例
	--"Hotfix.FixLockStepMgr"
}

-- 注册所有需要修复的脚本
function this.Start()
	for _,v in ipairs(this.modules) do
		print('Hotfix:' .. v)
		local hotfix_module = reimport(v)
		hotfix_module.Register()
	end
end

-- 取消注册
function this.Stop()
	for _,v in ipairs(this.modules) do
		local hotfix_module = require(v)
		hotfix_module.UnRegister()
	end
end

this.Start()