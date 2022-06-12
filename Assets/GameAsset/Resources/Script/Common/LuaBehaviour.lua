
--- Lua Behaviour

-- 示例
-- local Test = LuaBehaviour:new()
-- function Test.awake(go)
--     print(go.name)
-- end
-- function Test.start()
--     print(go.name)
-- end

-- local go = GameObject("test1")
-- CSBehaviour.AddComponent(go,Test)



LuaBehaviour = {}
function LuaBehaviour:new()
    
    local o = {}
    setmetatable(o, {__index = self})
    return o
end



