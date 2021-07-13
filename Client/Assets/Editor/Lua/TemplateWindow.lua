%Template% = {}

%Template%.sysUseBlackBg = false
%Template%.sysIsFullWindow = false 
%Template%.sysIs3DWindow = false 
%Template%.sysShowReturn = false 
%Template%.sysRuleId = nil 
%Template%.sysReturnFunc = nil 
%Template%.sysTitleLang = nil 
%Template%.sysMainWindow = false 
%Template%.sysCurrencyList = nil 


function %Template%:OnOpen(param) --param为OpenWindow时传入的第二个参数

end

--界面关闭时调用该方法 可写可不写
--function %Template%:OnClose() 
--
--end


%Template% = ToLuaClass.Class(UIWindow, {}, %Template%)