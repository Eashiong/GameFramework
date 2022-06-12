--[[
FilePath: /xlua_v2.1.15/Assets/Resources/Script/Common/LuaMain.lua
Description: Lua 逻辑主入口
--]]



local LuaMain = {}
local this = LuaMain
json = {}
function this.Main()
    this.FirstRequire()
    local game = require 'Main.GameStart'
    game.Start()
end

function this.FirstRequire()
    -- 基础包
    require 'Common.Reimport'
    require 'Common.luaEvent'
    json = require "Common.json"
    require 'Common.cs_coroutine'
    require 'Common.ObjectPool'
    require 'Common.LuaBehaviour'
    require 'Main.Define'
    require 'Common.functions'
    require 'Main.CommHotfix'


    --ui mvc
    require 'Panel.BasePanel'
    require 'Panel.BasePanelCtrl'




end

this.Main()
