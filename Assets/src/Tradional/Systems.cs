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
                Module.Score score = new Module.Score();

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
                    if (Vector3.Magnitude(obj.GetComponent<NavMeshAgent>().velocity) <= 0.0001f)
                        obj.GetComponent<NavMeshAgent>().SetDestination(GameMananger.RandomNavmeshLocation(40f, obj));
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

                    new EntityQuery()
                    .With<Module.Score>()
                    .ForEach(objToFollow =>
                    {
                        if(count == following)
                        {
                            follow.target = objToFollow;
                        }

                        count++;
                    });
                });
        }
    };


}
