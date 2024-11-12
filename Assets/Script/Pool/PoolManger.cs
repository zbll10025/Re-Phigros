using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManger
{
    public static PoolManger _instance;
    public Dictionary<string, Pool> dic_Pool = new Dictionary<string, Pool>();
    public static PoolManger Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PoolManger();
            }

            return _instance;
        }
    }

    public GameObject Get(string name, string path)
    {
        if (!dic_Pool.TryGetValue(name, out var pool))
        {
            pool = new Pool();
            dic_Pool.Add(name, pool);
        }
        return pool.Get(path);
        //被调用后要激活gamobject
    }

    public void Recycle(string name, GameObject a)
    {
        if (dic_Pool.TryGetValue(name, out var pool))
        {

            dic_Pool[name].Recycle(a);
        }


    }

    public void ClearPool(string name)
    {
        if (dic_Pool.TryGetValue(name, out var pool))
        {
            dic_Pool[name].ClearPool();
        }
    }


    public void CreatPool(string name, string path, int max)
    {
        Pool pool = new Pool();
        dic_Pool.Add(name, pool);
        pool.Creatpool(path, max);
    }
}
