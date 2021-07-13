using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : Singleton<AssetManager>
{
    public GameObject LoadGameObject(string path)
    {
        GameObject asset = Resources.Load<GameObject>(path);
        if (asset != null)
        {
            return GameObject.Instantiate(asset);
        }
        return null;
    }
}
