---@class AssetManager : Singleton
local m = {}
---@param path string
---@return UnityEngine.GameObject
function m:LoadGameObject(path) end
AssetManager = m
return m