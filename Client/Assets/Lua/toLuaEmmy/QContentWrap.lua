---@class QContent : UnityEngine.MonoBehaviour
---@field rect UnityEngine.RectTransform
---@field qScroll QScroll
---@field qCellEntity UnityEngine.GameObject
---@field pool QEntityPool
---@field qCells table
---@field updateChildrenCallback QContent.UpdateChildrenCallbackDelegate
---@field init QContent.InitDelegate
local m = {}
---@param data UnityEngine.Vector2
function m:SetScrollAmount(data) end
---@param amount int
function m:SetAmount(amount) end
QContent = m
return m