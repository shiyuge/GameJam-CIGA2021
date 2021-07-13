ToLuaClass={}

Super = function( self, curStaticClass, funcKey, ... )
    -- self可能是子类实例，所以先找到当前类实例
    local currentInstance = self

    while currentInstance.__class ~= curStaticClass.__class and currentInstance.__base do
        currentInstance = currentInstance.__base
    end

    --从当前类的实例开始往继承链顶部查找
    local parentInstance = currentInstance.__base
    while parentInstance ~= nil and type(parentInstance) == "table" do
        if parentInstance.__class and rawget(parentInstance.__class,funcKey) then

            parentInstance[funcKey](self,...)
            break
        else
            parentInstance = parentInstance.__base
        end
    end
end

function ToLuaClassInstance(table)
    --print("XluaClassInstance....")
    return table()
end

local SetField = function(t,k,v)
    local found = false
    local lookUpInstance = t
    while lookUpInstance ~= nil and type(lookUpInstance) == "table" do
        if lookUpInstance.__class and rawget(lookUpInstance.__class,k) then
            rawset( lookUpInstance, k, v )
            found = true
            break
        else
            lookUpInstance = lookUpInstance.__base
        end
    end

    if not found then
        rawset(t,k,v)
    end
end

function ToLuaClass.Class(base,static,instance)
    local baseMt = getmetatable(base)
    local class = static or {}
    class.__class = instance
    setmetatable(class,{
        __index = base,
        __call = function(...)

            local baseInstance = baseMt  and baseMt.__call and baseMt.__call(...)
            local instanceClass = instance or {}

            instanceClass["setItem"] = function(self,key,value)
                self[key] = value
            end

            instanceClass["getItem"] = function(self,key)
                return self[key]
            end

            local newInstance = setmetatable(
                    {
                        __base = baseInstance,
                        __class = instanceClass,
                    },
                    {
                        __index = function(t, k)
                            local ret_field
                            ret_field = instanceClass[k]
                            if nil == ret_field and baseInstance then
                                ret_field = baseInstance[k]
                            end

                            return ret_field
                        end,

                        __newindex = SetField
                    })

            if instanceClass.ctor then
                instanceClass.ctor(newInstance, ...)
            end

            return newInstance
        end,
        initFunc =__call
    })

    if rawget(class,'static_ctor') then
        class:static_ctor()
    end
    return class
end