using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QEntityPool
{
    public Stack<GameObject> pool;
    public GameObject entity;
    public List<GameObject> record;
    public QEntityPool(GameObject e)
    {
        pool = new Stack<GameObject>();
        record = new List<GameObject>();
        entity = e;
    }

    public GameObject Get()
    {
        if (pool.Count >= 1)
        {
            GameObject go = pool.Pop();
            go.SetActive(true);
            return go;
        }
        else
        {
            GameObject go = GameObject.Instantiate(entity, entity.transform.parent);
            go.name = go.name + (record.Count);
            record.Add(go);
            go.SetActive(true);
            return go;
        }
    }

    public void Recycle(GameObject go)
    {
        go.SetActive(false);
        pool.Push(go);
    }
}
