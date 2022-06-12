--[[
    UI 面板基类
]]
basePanel = {name = nil}
function basePanel:new()
    local o = {}
    setmetatable(o, {__index = self})
    return o
end

--[[
    @desc: 创建UI到场景
    @return:void
]]
function basePanel:Build()
    self.rootObj = self.Builder()
    self.root = self.rootObj.transform
    self.name = self.rootObj.name
    self:OnInit()
end
--[[
    @desc: 从场景销毁UI
    @return:void
]]
function basePanel:Kill()
    if self:Exist() == false then
        print("UI不存在")
        return
    end

    if self.OnKill ~= nil then
        self.OnKill()
    end
    GameObject.Destroy(self.rootObj)
    self.rootObj = nil
    self.root = nil
    self.name = nil
end
--[[
    @desc: UI存在
    @return:如果存在返回true
]]
function basePanel:Exist()
    return self.rootObj ~= nil
end
--[[
    @desc: 刷新界面
    @return:如果存在返回true
]]
function basePanel:Reflesh(data)
    if self:Exist() == false then
        print("UI不存在")
        return
    end
    if self.OnReflesh ~= nil then
        self.OnReflesh(data)
    end
end
--[[
    @desc: 隐藏
    @return:void
]]
function basePanel:Hide()
    if self:Exist() == false then
        print("UI不存在")
        return
    end
    if self.OnHide ~= nil then
        self.OnHide()
    end
    self.rootObj:SetActive(false)
end
--[[
    @desc: 显示
    @return:void
]]
function basePanel:Show()
    if self:Exist() == false then
        print("UI不存在")
        return
    end
    if self.OnShow ~= nil then
        self.OnShow()
    end
    self.rootObj:SetActive(true)
end
