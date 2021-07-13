// using UnityEngine;
using System.Collections;

public class Singleton<T> where T : class, new()
{
    private static T _instance;
    private static readonly object syslock = new object();
    protected Singleton()
    {
    }

    public static T Instance
    {
        get
        {
            if (_instance == null) {
                lock (syslock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                }
            }
            return _instance;
        }
    }

    public static void DestoryInstance()
    {
        if (_instance != null)
        {
            _instance = null;
        }
    }
}
