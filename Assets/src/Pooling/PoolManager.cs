using System;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    Player,
    Ennemy
}

[Serializable]
public struct ObjectForPoll
{
    public ObjectType objectType;
    public PoolObject poolObject;
    public int number;
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance() { return _singleton; }
    private static PoolManager _singleton;

    public List<ObjectForPoll> list;

    private Dictionary<ObjectType, Pool> poolDic = new Dictionary<ObjectType, Pool>();

    private void Awake()
    {
        _singleton = this;
    }

    public void createPools()
    {
        for (int i = 0; i < list.Count; i++)
        {
            Pool pool = new Pool();
            pool.initialize(list[i].poolObject, list[i].number);
            poolDic.Add(list[i].objectType, pool);
        }
    }

    public PoolObject GetPooledObject(ObjectType objectType)
    {
        return poolDic[objectType].getObject();
    }

    public void ReleasePooledObject(PoolObject poolObject)
    {
        poolObject.setStatus(false);
    }

}
