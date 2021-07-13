UIManager = {}
local Canvas = LoadGameObject("Prefabs/Canvas")
UnityEngine.Object.DontDestroyOnLoad(Canvas)

local curWindow

---@param path string
OpenWindow = function(path)
    local windowGo = LoadGameObject(path)
    windowGo.transform.parent = Canvas.transform
    windowGo.transform.localPosition = Vector3(0,0,0)
    windowGo.transform.localScale = Vector3.one
    curWindow = windowGo
end

CloseCurWindow = function()
    if curWindow then
        GameObject.Destroy(curWindow)
        curWindow = nil
    end
end