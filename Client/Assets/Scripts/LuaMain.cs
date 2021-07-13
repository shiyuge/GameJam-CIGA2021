using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.IO;


using LuaInterface;

public partial class LuaMain : Singleton<LuaMain>
{
    public LuaState lua
    {
        get
        {
            return _lua;
        }

        set
        {
            _lua = value;
        }
    }


    LuaState _lua;
    private LuaLooper loop = null;
    public void StartLua<T>() where T : MonoBehaviour
    {
        lua = new LuaState();
        lua.LuaSetTop(0);
        LuaBinder.Bind(lua);
        DelegateFactory.Init();
        MonoBehaviour gameLoop = GameObject.FindObjectOfType<T>();
        LuaCoroutine.Register(lua, gameLoop);
        loop = gameLoop.gameObject.AddComponent<LuaLooper>();
        loop.luaState = lua;
        this.lua.Start();

        lua.DoFile("main.lua");
    }

    public LuaTable CreateTable()
    {
        return lua.NewTable();
    }

    public LuaTable GetTable(string path)
    {
        var table = lua.GetTable(path);
        if (table == null)
        {
        }
        return table;
    }
    public static LuaTable Instantiate(LuaTable cls)
    {

        LuaFunction function = LuaMain.Instance.lua.GetFunction("ToLuaClassInstance");
        if (function != null)
        {
            LuaTable returnTable = function.Invoke<LuaTable, LuaTable>(cls);
            return returnTable;
        }

        return null;
    }

    public static LuaFunction GetMethod(LuaTable cls, string name)
    {
        return cls.GetLuaFunction(name);
    }
}
