GameObject = UnityEngine.GameObject
Camera = UnityEngine.Camera

---@param path string
---@return UnityEngine.GameObject
function LoadGameObject(path)
	return AssetManager.Instance:LoadGameObject(path)
end

require("Common/ToLuaClass")
require("Common/UIManager")
require("Common/SceneManager")
require("Common/FirstInitManager")

--主入口函数。从这里开始lua逻辑
function Main()					
	print("logic start")	 		
end

function OnApplicationQuit()
end

GameObject.Find("TestText"):GetComponent("Text").text = "success"