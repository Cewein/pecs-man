using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Systems;
using ECS;

public class GameMananger : MonoBehaviour
{
    public int nbPecsMan;
    public int nbFoods;
    public int nbEnemy;

    public GameObject PecsMan;
    public GameObject Food;
    public GameObject Enemy;

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

    void Awake()
    {
        ECSSystem.AddSystem(new PecsManSystem(PecsMan, nbPecsMan));
        ECSSystem.AddSystem(new FoodSystem(Food, nbFoods));
        ECSSystem.AddSystem(new EnemySystem(Enemy, nbEnemy));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


