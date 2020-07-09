using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ECS;

namespace Systems
{
    class PecsManSystem : ECS.ECSSystem
    {
        private GameObject prefab;
        public int number;

        public PecsManSystem(GameObject prefab, int number)
        {
            this.prefab = prefab ?? throw new ArgumentNullException(nameof(prefab));
            this.number = number;
        }

        void Create()
        {
            for (int i = 0; i < number; i++)
            {
                //component
                Module.TargetEdible miam = new Module.TargetEdible();
                Module.Score score = new Module.Score(i, 0, false);

                //entity
                GameObject tmp = ECS.EntityActionBuffer.Instance.CreateEntity(prefab);
                tmp.transform.position = GameMananger.RandomNavmeshLocation(40f, tmp);

                //merging both
                ECS.EntityActionBuffer.Instance.AddComponent(tmp, miam);
                ECS.EntityActionBuffer.Instance.AddComponent(tmp, score);
            }
        }

        public override void ExecuteOnce()
        {
            Create();
        }

        public override void Update()
        {
            new EntityQuery()
                .With<Module.TargetEdible>()
                .With<Module.Score>()
                .ForEach(obj =>
                {
                    Module.TargetEdible food = obj.GetECSComponent<Module.TargetEdible>();
                    
                    float dist = float.MaxValue;

                    new EntityQuery()
                        .With<Module.Edible>()
                        .ForEach(pocky =>
                        {
                            if (Vector3.Distance(obj.transform.position, pocky.transform.position) < dist)
                            {
                                food.target = pocky;
                                dist = Vector3.Distance(obj.transform.position, pocky.transform.position);
                            }
                        });

                    EntityActionBuffer.Instance.ApplyComponentChanges<Module.TargetEdible>(obj, food);
                    

                    if (Vector3.Distance(obj.transform.position, food.target.transform.position) <= 1.2f)
                    {
                        Module.Edible pocky = food.target.GetECSComponent<Module.Edible>();
                        pocky.active = false;
                        EntityActionBuffer.Instance.ApplyComponentChanges(food.target, pocky);

                        Module.Score score = obj.GetECSComponent<Module.Score>();
                        score.score += 1;
                        EntityActionBuffer.Instance.ApplyComponentChanges(obj, score);
                    }

                    obj.GetComponent<NavMeshAgent>().SetDestination(food.target.transform.position);

                    if (obj.GetECSComponent<Module.Score>().isDead)
                    {
                        obj.transform.position = GameMananger.RandomNavmeshLocation(40f, obj);
                        obj.GetComponent<TrailRenderer>().Clear();

                        Module.Score score = obj.GetECSComponent<Module.Score>();

                        MonoBehaviour.print($"pecsman N°{score.number} with score of {score.score}");

                        score.score = 0;
                        score.isDead = false;
                        EntityActionBuffer.Instance.ApplyComponentChanges(obj, score);

                    }


                });
        }
    };

    class FoodSystem : ECS.ECSSystem
    {
        private GameObject prefab;
        private int number;

        public FoodSystem(GameObject prefab, int number)
        {
            this.prefab = prefab ?? throw new ArgumentNullException(nameof(prefab));
            this.number = number;
        }

        void Create()
        {
            for (int i = 0; i < number; i++)
            {
                //component
                Module.Edible edible = new Module.Edible();

                //entity
                GameObject tmp = ECS.EntityActionBuffer.Instance.CreateEntity(prefab);
                tmp.transform.position = GameMananger.RandomNavmeshLocation(40, tmp);

                //merging both
                ECS.EntityActionBuffer.Instance.AddComponent(tmp, edible);
            }
        }

        public override void ExecuteOnce()
        {
            Create();
        }

        public override void Update()
        {
            new EntityQuery()
                .With<Module.Edible>()
                .ForEach(obj =>
                {
                    Module.Edible edible = obj.GetECSComponent<Module.Edible>();
                    if (!edible.active)
                    {
                        obj.transform.position = GameMananger.RandomNavmeshLocation(40f, obj);
                        edible.active = true;
                        EntityActionBuffer.Instance.ApplyComponentChanges(obj, edible);
                    }
                });
        }
    };

    class EnemySystem : ECS.ECSSystem
    {
        private GameObject prefab;
        private int number;

        public EnemySystem(GameObject prefab, int number)
        {
            this.prefab = prefab ?? throw new ArgumentNullException(nameof(prefab));
            this.number = number;
        }

        void Create()
        {
            for (int i = 0; i < number; i++)
            {
                //component
                Module.FollowTarget follow = new Module.FollowTarget();

                //entity
                GameObject tmp = ECS.EntityActionBuffer.Instance.CreateEntity(prefab);
                tmp.transform.position = GameMananger.RandomNavmeshLocation(40, tmp);

                //merging both
                ECS.EntityActionBuffer.Instance.AddComponent(tmp, follow);
            }
        }

        public override void ExecuteOnce()
        {
            Create();
        }

        public override void Update()
        {
            new EntityQuery()
                .With<Module.FollowTarget>()
                .ForEach(obj =>
                {

                    int following = UnityEngine.Random.Range(0, GameMananger.globalnbPecsMan);
                    int count = 0;

                    Module.FollowTarget follow = obj.GetECSComponent<Module.FollowTarget>();

                    //we pick a new target
                    if (follow.target == null || follow.target.GetECSComponent<Module.Score>().isDead)
                    {
                        new EntityQuery()
                        .With<Module.Score>()
                        .ForEach(objToFollow =>
                        {
                            if (count == following)
                            {
                                follow.target = objToFollow;
                            }

                            count++;
                        });
                        EntityActionBuffer.Instance.ApplyComponentChanges(obj, follow);
                    }

                    //We see if the target is dead or not
                    if(Vector3.Distance(obj.transform.position, follow.target.transform.position) <= 1.0f)
                    {
                        Module.Score score = follow.target.GetECSComponent<Module.Score>();
                        score.isDead = true;
                        EntityActionBuffer.Instance.ApplyComponentChanges(follow.target, score);
                    }

                    obj.GetComponent<NavMeshAgent>().SetDestination(follow.target.transform.position);

                });
        }
    };


}
