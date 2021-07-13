using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoop : MonoBehaviour
{
    void Start()
    {
        LuaMain.Instance.StartLua<GameLoop>();
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        LuaMain.DestoryInstance();
    }

    void Update()
    {

    }
}
