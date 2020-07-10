using ECS;
using Module.Components;
using UnityEngine;
using UnityEngine.AI;

namespace Tradional.Systems
{
	public class EnemySystem : ECSSystem
    {
        private GameObject _prefab;
        private int _number;

        public EnemySystem(GameObject prefab, int number)
        {
            _prefab = prefab;
            _number = number;
        }

        private void Create()
        {
            for (int i = 0; i < _number; i++)
            {
                //component
                FollowTarget follow = new FollowTarget(null,false);

                //entity
                GameObject tmp = EntityActionBuffer.Instance.CreateEntity(_prefab);
                tmp.transform.position = GameMananger.RandomNavmeshLocation(400, tmp);

                //merging both
                EntityActionBuffer.Instance.AddComponent(tmp, follow);
            }
        }

        public override void ExecuteOnce()
        {
            Create();
        }

        public override void Update()
        {
            new EntityQuery()
                .With<FollowTarget>()
                .ForEach(obj =>
                {
                    FollowTarget follow = obj.GetECSComponent<FollowTarget>();

                    //calming down the enemy
                    if (follow.HasBeenCalmDown)
                    {
                        obj.transform.position = GameMananger.RandomNavmeshLocation(400f, obj);
                        obj.GetComponent<TrailRenderer>().Clear();

                        follow.HasBeenCalmDown = false;

                        EntityActionBuffer.Instance.ApplyComponentChanges(obj, follow);
                    }

                    //we pick a new target
                    PickNewTarget(obj, follow);

                    //We see if the target is dead or not
                    KillTarget(obj, follow);

                    if(follow.Target != null)
                        obj.GetComponent<NavMeshAgent>().SetDestination(follow.Target.transform.position);

                });
        }

        private void PickNewTarget(GameObject obj, FollowTarget follow)
        {
            //only pick new target once or if the new target want to do a Vendetta
            //it will chase it down until he is dead
            if (follow.Target == null || follow.Target.GetECSComponent<Vendetta>().WantToDoVendetta)
            {
                float dist = float.MaxValue;
                new EntityQuery()
                .With<Score>()
                .With<Vendetta>()
                .ForEach(objToFollow =>
                {
                    //only pick green pecsman
                    Vendetta vendetta = objToFollow.GetECSComponent<Vendetta>();
                    if (!vendetta.WantToDoVendetta)
                    {
                        if (Vector3.Distance(obj.transform.position, objToFollow.transform.position) < dist)
                        {
                            follow.Target = objToFollow;
                            dist = Vector3.Distance(obj.transform.position, objToFollow.transform.position);
                        }
                    }
                });
                EntityActionBuffer.Instance.ApplyComponentChanges(obj, follow);
            }
        }

        private void KillTarget(GameObject obj, FollowTarget follow)
        {
            if (follow.Target != null && Vector3.Distance(obj.transform.position, follow.Target.transform.position) <= 1.0f)
            {
                EntityActionBuffer.Instance.ApplyComponentChanges(obj, follow);

                Score score = follow.Target.GetECSComponent<Score>();
                score.IsDead = true;
                EntityActionBuffer.Instance.ApplyComponentChanges(follow.Target, score);

                //we set the target of the dead pecsman to the killer
                Vendetta vendetta = follow.Target.GetECSComponent<Vendetta>();
                vendetta.WantToDoVendetta = true;
                vendetta.Target = obj;

                follow.Target.GetComponent<MeshRenderer>().material.color = Color.magenta;
                follow.Target.GetComponent<TrailRenderer>().material.color = Color.magenta;
                follow.Target.GetComponent<NavMeshAgent>().speed = 10.0f;

                EntityActionBuffer.Instance.ApplyComponentChanges(follow.Target, vendetta);
            }
        }
    }
}