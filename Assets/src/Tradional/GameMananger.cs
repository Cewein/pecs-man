using ECS;
using Tradional.Systems;
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

    public static Vector3 RandomNavmeshLocation(float radius, GameObject entity)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += entity.transform.position;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    private void Awake()
    {
        ECSSystem.AddSystem(new PecsManSystem(PecsMan, nbPecsMan));
        ECSSystem.AddSystem(new FoodSystem(Food, nbFoods));
        ECSSystem.AddSystem(new EnemySystem(Enemy, nbEnemy));
    }
}


