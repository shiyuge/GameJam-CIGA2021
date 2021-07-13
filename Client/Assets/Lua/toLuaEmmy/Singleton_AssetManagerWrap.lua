---@class Singleton<AssetManager> : object
---@field Instance AssetManager
local m = {}
function m.DestoryInstance() end
Singleton<AssetManager> = m
return m