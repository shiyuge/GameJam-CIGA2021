using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;

public class LuaBehaviour : MonoBehaviour
{
    [LuaInterface.NoToLua]
    public delegate void RebindCallback();
    public RebindCallback rebindCallback;

    private LuaTable self;
    private LuaTable cls;
    private Dictionary<string, LuaFunction> methods = new Dictionary<string, LuaFunction>();

    [LuaInterface.NoToLua]
    public string fileName;
    string className;


    void Start()
    {
        if (self != null) {return;}
        Init();
    }

    void Init()
    {
        if (string.IsNullOrEmpty(fileName)) {return;}

        string[] path = fileName.Split(new char[] { '/' });
        className = path[path.Length - 1];

        cls = LuaMain.Instance.GetTable(className);
        if (cls == null)
        {
            LuaMain.Instance.lua.DoFile(fileName);
            cls = LuaMain.Instance.GetTable(className);
        }
        self = LuaMain.Instantiate(cls) as LuaTable;

        self["gameObject"] = gameObject;
        self["transform"] = transform;

        if (rebindCallback != null)
        {
            rebindCallback();
            rebindCallback = null;
        }

        Binder(self);

        CallMethod("Start");
    }

    private void Binder(LuaTable self)
    {
        ComponentBinderEx binderEx = gameObject.GetComponent<ComponentBinderEx>();
        if (binderEx != null)
        {
            binderEx.SetLuaData(self);
        }
    }

    void Update()
    {
        CallMethod("Update");
    }

    void OnDestroy()
    {
        if (cls != null)
        {
            cls.Dispose();
        }
        cls = null;
        if (self != null)
        {
            self.Dispose();
        }
        self = null;
        methods.Clear();
    }
    public LuaTable GetLuaObject()
    {
        if (self == null && !gameObject.activeSelf)
        {
            Init();
        }
        return self;
    }

    public void CallMethod(string methodName)
    {
        if (self == null)
            return;
        LuaFunction method;
        if (!methods.TryGetValue(methodName, out method))
            methods.Add(methodName, method = LuaMain.GetMethod(self, methodName));
        if (method != null) method.Call<LuaTable>(self);
    }
}
