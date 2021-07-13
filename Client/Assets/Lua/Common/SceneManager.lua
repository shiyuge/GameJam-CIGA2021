require("Scenes/GameMenuScene")
require("Scenes/CabinScene")
SceneManager = {}

local sceneList = {
    GameMenu = GameMenuScene,
    Cabin = CabinScene,
}

local curScene

---@param path string
LoadScene = function(path)
    UnityEngine.SceneManagement.SceneManager.LoadScene(path)
    if curScene then
        curScene.OnExit()
    end
    curScene = sceneList[path]
    curScene.OnEnter()
end

---第一个场景
LoadScene("GameMenu")