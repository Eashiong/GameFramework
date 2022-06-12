--事件分发

-- local function func(a, b)
--     print(a, b)
-- end
-- local function func2(a)
--     print(a)
-- end

-- luaEvent.Add("Test", func)
-- luaEvent.Add("Test", func2)
-- luaEvent.Remove("Test", func2)
-- luaEvent.Add("Test2", func2)
-- luaEvent.debug()

luaEvent = {}
local this = luaEvent

this.events = {}

function luaEvent.Add(eventName, func)
    if (this.events[eventName] == nil) then
        this.events[eventName] = {}
    end
    table.insert(this.events[eventName], {enable = true, fun = func})
end
function luaEvent.Remove(eventName, func)
    for k, v in ipairs(this.events[eventName]) do
        if (v.fun == func) then
            v.enable = false
            v.fun = nil
        end
    end
end
function luaEvent.Send(eventName, ...)
    if (this.events[eventName] ~= nil) then
        for i, v in ipairs(this.events[eventName]) do
            if (v.enable == true) then
                v.fun(...)
            else
                --print("事件被叫停")
            end
            
        end
    end
end
function luaEvent.debug()
    print("所有事件信息 start")
    for i,event in pairs(this.events) do
        print('函数名:'.. i)
        for j, funTable in ipairs(event) do
            print(tostring(funTable.fun) .. '  enable:' .. tostring(funTable.enable))
        end
        print('------------------------')
        print()
    end
    print("所有事件信息 end")
        
end



