--[[
    
    UI控制器 并且整合了Model
]]
basePanelCtrl = {panel = nil, data = nil}
function basePanelCtrl:new()
    local o = {}
    setmetatable(o, {__index = self})
    return o
end
--[[
    @desc: 创建一个Panel 如果有默认数据则刷新界面
    @data: 默认数据
    @return:void
]]
function basePanelCtrl:Create(buildData)

    self.data = self.DataGetter(buildData)
    
    if self:Exist() then
        print("UI重复创建")
        if  self.data then
            self:Reflesh()
        end
        return
    end
    self.panel = self.PanelGetter()
   
    self.panel:Build()
    if  self.data then
        self:Reflesh()
    end
end
--[[
    @desc: 重绘panel
    @return:void
]]
function basePanelCtrl:SetData(data, dontReflesh)
    if not self:Exist() then
        print("UI不存在")
        return
    end
    self.data = data or {}
    if dontReflesh == nil or dontReflesh == false then
        self:Reflesh()
    end
end
--[[
    @desc: 取数据
    @return:data
]]
function basePanelCtrl:GetData()
    return self.data
end
--[[
    @desc: 取面板
    @return:panel
]]
function basePanelCtrl:GetPanel()
    return self.panel
end
function basePanelCtrl:Show(show)
    if not self:Exist() then
        print("UI不存在")
        return
    end
    if show == nil then
        show = true
    end
    if show then
        self.panel:Show()
    else
        self.panel:Hide()
    end
     
end
--[[
    @desc: 内部方法
    @return:void
]]
function basePanelCtrl:Reflesh()
    if not self:Exist() then
        print("UI不存在")
        return
    end
    self.panel:Reflesh(self.data)
end
--[[
    @desc: 关闭panel
    @return:void
]]
function basePanelCtrl:Kill()
    if not self:Exist() then
        print("UI不存在")
        return
    end
    if self.OnDispose ~= nil then
        self.OnDispose()
    end
    self.panel:Kill()
    self.panel = nil
end
--[[
    @desc: panel存在判断
    @return:存在返回true
]]
function basePanelCtrl:Exist()
    return self.panel ~= nil
end
