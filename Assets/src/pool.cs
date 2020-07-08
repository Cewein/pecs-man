using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool
{
    private PoolObject[] pool;
    private int size;

    public void initialize(PoolObject poolObject, int size)
    {
        pool = new PoolObject[size];
        this.size = size;
        for(int i = 0; i < size; i++)
        {
            pool[i] = Object.Instantiate(poolObject);
            pool[i].setStatus(false);
        }
    }

    public PoolObject getObject()
    {
        for (int i = 0; i < size; i++)
        {
            if(!pool[i].state)
            {
                return pool[i];
            }
        }

        return null;
    }

    void Start()
    {
        
    }

}

