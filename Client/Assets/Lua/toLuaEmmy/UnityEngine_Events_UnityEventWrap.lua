---@class UnityEngine.Events.UnityEvent : UnityEngine.Events.UnityEventBase
local m = {}
---@param call UnityEngine.Events.UnityAction
function m:AddListener(call) end
---@param call UnityEngine.Events.UnityAction
function m:RemoveListener(call) end
function m:Invoke() end
UnityEngine = {}
UnityEngine.Events = {}
UnityEngine.Events.UnityEvent = m
return m