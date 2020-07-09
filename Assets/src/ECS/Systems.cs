﻿using System;
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
        private int number;

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
            EntityQuery entityQuery = new EntityQuery();
            entityQuery = entityQuery.WithComponent<Module.TargetEdible>().WithComponent<Module.Score>();
            entityQuery.ForEach(obj =>
            {
                if(Vector3.Magnitude(obj.GetComponent<NavMeshAgent>().velocity) <= 0.0001f)
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
    };


}
