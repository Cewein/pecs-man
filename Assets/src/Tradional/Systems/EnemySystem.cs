using System;
using ECS;
using Module;
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
                    if (follow.asBeenCalmDown)
                    {
                        obj.transform.position = GameMananger.RandomNavmeshLocation(400f, obj);
                        obj.GetComponent<TrailRenderer>().Clear();

                        follow.asBeenCalmDown = false;

                        EntityActionBuffer.Instance.ApplyComponentChanges(obj, follow);
                    }

                    //we pick a new target
                    PickNewTarget(obj, follow);

                    //We see if the target is dead or not
                    KillTarget(obj, follow);

                    if(follow.target != null)
                        obj.GetComponent<NavMeshAgent>().SetDestination(follow.target.transform.position);

                });
        }

        private void PickNewTarget(GameObject obj, FollowTarget follow)
        {
            //only pick new target once or if the new target want to do a Vendetta
            //it will chase it down until he is dead
            if (follow.target == null || follow.target.GetECSComponent<Vendetta>().wantToDoVendetta)
            {
                float dist = float.MaxValue;
                new EntityQuery()
                .With<Score>()
                .With<Vendetta>()
                .ForEach(objToFollow =>
                {
                    //only pick green pecsman
                    Vendetta vendetta = objToFollow.GetECSComponent<Vendetta>();
                    if (!vendetta.wantToDoVendetta)
                    {
                        if (Vector3.Distance(obj.transform.position, objToFollow.transform.position) < dist)
                        {
                            follow.target = objToFollow;
                            dist = Vector3.Distance(obj.transform.position, objToFollow.transform.position);
                        }
                    }
                });
                EntityActionBuffer.Instance.ApplyComponentChanges(obj, follow);
            }
        }

        private void KillTarget(GameObject obj, FollowTarget follow)
        {
            if (follow.target != null && Vector3.Distance(obj.transform.position, follow.target.transform.position) <= 1.0f)
            {
                EntityActionBuffer.Instance.ApplyComponentChanges(obj, follow);

                Score score = follow.target.GetECSComponent<Score>();
                score.isDead = true;
                EntityActionBuffer.Instance.ApplyComponentChanges(follow.target, score);

                //we set the target of the dead pecsman to the killer
                Vendetta vendetta = follow.target.GetECSComponent<Vendetta>();
                vendetta.wantToDoVendetta = true;
                vendetta.target = obj;

                follow.target.GetComponent<MeshRenderer>().material.color = Color.magenta;
                follow.target.GetComponent<TrailRenderer>().material.color = Color.magenta;
                follow.target.GetComponent<NavMeshAgent>().speed = 10.0f;

                EntityActionBuffer.Instance.ApplyComponentChanges(follow.target, vendetta);
            }
        }
    }
}