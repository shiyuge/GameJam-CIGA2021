----------COMPONENTS BEGIN----------
---@class ScrollTemplate
---@field QScroll Transform
---@field QContent Transform
----------COMPONENTS END----------
ScrollTemplate = {}

function ScrollTemplate:Start()
    local qContent = self.QContent:GetComponent("QContent")
    qContent:SetAmount(8)
end

ScrollTemplate = ToLuaClass.Class({},{}, ScrollTemplate)
