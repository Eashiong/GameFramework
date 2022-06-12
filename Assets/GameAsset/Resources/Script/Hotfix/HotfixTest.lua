-- --[[
-- FilePath: /xlua_v2.1.15/Assets/Resources/Script/Hotfix/HotfixTest.lua
-- Description: 
-- --]]

-- -- 这只是一个使用案例测试

-- local HotfixTest = { }
-- local this = HotfixTest
-- this.PropertyTest = {}


-- local function Ctor()
--     print("new MyTest()  by lua")
--     local cube = GameObject.Find('Cube CS');
--     GameObject.Destroy(cube);
--     local sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
--     sphere.name = 'Sphere lua';
-- end

-- local function Get_PropertyTest()

--     print("PropertyTest getter by lua")
--     return this.PropertyTest
-- end

-- local function Set_PropertyTest(value)

--     print("PropertyTest setter by lua")
--     this.PropertyTest = value
-- end

-- local function FunTest()
--     print("FunTest by lua")
-- end
-- local function StaticFunTest()
--     print("StaticFunTest by lua")
-- end


-- -- 所有的热修复脚本都要实现注册初始化逻辑
-- function this.Register()
--     xlua.hotfix(CS.MyTest, 'FunTest', FunTest)
--     xlua.hotfix(CS.MyTest, 'StaticFunTest', StaticFunTest)    
--     xlua.hotfix(CS.MyTest, '.ctor', Ctor)
--     xlua.hotfix(CS.MyTest, 'get_PropertyTest', Get_PropertyTest)
--     xlua.hotfix(CS.MyTest, 'set_PropertyTest', Set_PropertyTest)    
-- end

-- -- 所有的热修复脚本都要实现销毁逻辑
-- function this.UnRegister()
--     xlua.hotfix(CS.MyTest, 'FunTest', nil)
--     xlua.hotfix(CS.MyTest, 'ctor', nil)
--     xlua.hotfix(CS.MyTest, 'get_PropertyTest', nil)
--     xlua.hotfix(CS.MyTest, 'set_PropertyTest', nil)  
-- end

-- return HotfixTest
