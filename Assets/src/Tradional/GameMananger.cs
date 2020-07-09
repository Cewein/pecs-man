using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameMananger : MonoBehaviour
{
    public int nbPecsMan;
    public int nbFoods;
    public int nbEnemy;

    public GameObject PecsMan;
    public GameObject Food;
    public GameObject Enemy;

    void CreatePecsMan()
    {
        for (int i = 0; i < nbPecsMan; i++)
        {
            //component
            Module.TargetEdible miam = new Module.TargetEdible();
            Module.Score score = new Module.Score();

            //entity
            GameObject tmp = ECS.EntityActionBuffer.Instance.CreateEntity(PecsMan);
            tmp.transform.position = RandomNavmeshLocation(40, tmp);

            //merging both
            ECS.EntityActionBuffer.Instance.AddComponent(tmp, miam);
            ECS.EntityActionBuffer.Instance.AddComponent(tmp, score);
        }
    }

    void CreateFood()
    {
        for (int i = 0; i < nbFoods; i++)
        {
            //component
            Module.Edible edible = new Module.Edible();

            //entity
            GameObject tmp = ECS.EntityActionBuffer.Instance.CreateEntity(Food);
            tmp.transform.position = RandomNavmeshLocation(40, tmp);

            //merging both
            ECS.EntityActionBuffer.Instance.AddComponent(tmp, edible);
        }
    }

    void CreateEnemy()
    {
        for (int i = 0; i <nbEnemy; i++)
        {
            //component
            Module.FollowTarget follow = new Module.FollowTarget();

            //entity
            GameObject tmp = ECS.EntityActionBuffer.Instance.CreateEntity(Enemy);
            tmp.transform.position = RandomNavmeshLocation(40, tmp);

            //merging both
            ECS.EntityActionBuffer.Instance.AddComponent(tmp, follow);
        }
    }

    public static Vector3 RandomNavmeshLocation(float radius, GameObject entity)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += entity.transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    void Start()
    {
        CreatePecsMan();
        CreateFood();
        CreateEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


