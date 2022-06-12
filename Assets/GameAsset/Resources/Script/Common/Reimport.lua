--[[
FilePath: /xlua_v2.1.15/Assets/Resources/Script/Common/Reimport.lua
Description: 
--]]


--重新require一个lua文件，替代系统文件。
function reimport(name)
    local package = package
    package.loaded[name] = nil
    package.preload[name] = nil
    return require(name)    
end
