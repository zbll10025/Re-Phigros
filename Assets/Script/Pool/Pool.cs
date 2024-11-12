using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Pool
{
    public GameObject prefabe;
    private Queue<GameObject> _pool = new Queue<GameObject>();
    public static int i = 0;
    public void Creatpool(string path, int max)
    {
        for (int i = 0; i < max; i++)
        {
            GameObject gameobject = CreatObject(path);
            gameobject.SetActive(false);
            if (gameobject != null)
            {
                _pool.Enqueue(gameobject);
            }
        }
    }

    public GameObject Get(string path)
    {

        if (_pool.Count != 0)
        {

            return _pool.Dequeue();

        }
        else
        {


            return CreatObject(path);
        }
    }

    public void Recycle(GameObject obj)
    {

        if (obj.GetComponent<Rigidbody2D>() != null)
        {
            obj.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }
        obj.SetActive(false);

        _pool.Enqueue(obj);


    }

    public void ClearPool()
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            var gameobject = _pool.Dequeue();
            // Destroy(gameobject);
        }
    }

    public GameObject CreatObject(string path)
    {


        return GameObject.Instantiate(Resources.Load(path)) as GameObject;
    }
}
