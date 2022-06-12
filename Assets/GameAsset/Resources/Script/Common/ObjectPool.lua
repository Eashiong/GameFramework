--- 对象池

local list = require "Common.list"

local ObjectPool = {
    prefab = nil,
    capacity = 0,
    cache = nil
}
-- 可重写函数
function ObjectPool.Spawn()
    return {}
end
function ObjectPool.Init(obj)
end
function ObjectPool.Restore(obj)
end
function ObjectPool.Release(obj)
end

function ObjectPool:New(capacity)
    local o = {}
    o.capacity = capacity
    o.cache = list:new()
    setmetatable(o, {__index = self})
    return o
end
function ObjectPool:InitCapacity()
    for i = 1, self.capacity do
        print(i)
        local obj = self.Spawn()
        self.cache:push(obj)
    end
end

function ObjectPool:Get()
    local obj = nil
    if self.cache.length > 0 then
        obj = self.cache:pop()
    else
        print("缓冲池满")
        obj = self.Spawn()
    end
    self.Init(obj)
    return obj
end
function ObjectPool:Recovery(obj)
    if self.cache.length < self.capacity then
        self.Restore(obj)
        self.cache:push(obj)
    else
        self.Release(obj)
    end
end

-- local testPoos = ObjectPool:New(20)
-- local root = GameObject("root").transform
-- function testPoos.Spawn()
--     local p = GameObject("pool")
--     p.transform:SetParent(root, false)
--     print("a")
--     return p
-- end
-- function testPoos.Init(obj)
--     obj.name = "init"
-- end
-- function testPoos.Restore(obj)
--     obj.name = "Restore"
-- end
-- function testPoos.Release(obj)
--     GameObject.Destroy(obj)
-- end
-- testPoos:InitCapacity()
-- for i=1,10 do
--     local v1 = testPoos:Get()
--     local v2 = testPoos:Get()
--     testPoos:Recovery(v1)
-- end

return list
