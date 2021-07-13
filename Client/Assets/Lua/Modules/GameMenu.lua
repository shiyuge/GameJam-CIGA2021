----------COMPONENTS BEGIN----------
---@class GameMenu
---@field btnStartGame Button
----------COMPONENTS END----------
GameMenu = {}
function GameMenu:Start()
    print("GameMenu:Start()")
    self.btnStartGame.onClick:AddListener(function()
        LoadScene("Cabin")
    end)

    --self.btnStartGame:GetComponent("Transform"):DOScale(2,5)
end

function GameMenu:Update()
end

GameMenu = ToLuaClass.Class({},{}, GameMenu)
