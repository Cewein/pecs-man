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
                Module.Vendetta vendetta = new Module.Vendetta(false, null);

                //entity
                GameObject tmp = ECS.EntityActionBuffer.Instance.CreateEntity(prefab);
                tmp.transform.position = GameMananger.RandomNavmeshLocation(40f, tmp);

                //merging both
                ECS.EntityActionBuffer.Instance.AddComponent(tmp, miam);
                ECS.EntityActionBuffer.Instance.AddComponent(tmp, score);
                EntityActionBuffer.Instance.AddComponent(tmp, vendetta);
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
                .With<Module.Vendetta>()
                .ForEach(obj =>
                {
                    Module.Vendetta vendetta = obj.GetECSComponent<Module.Vendetta>();

                    //pecsman death
                    Death(obj);

                    //normal behavior is not in vendetta state
                    if (!vendetta.wantToDoVendetta)
                    {
                        Module.TargetEdible food = obj.GetECSComponent<Module.TargetEdible>();

                        pickClosestFood(obj, food);

                        EatFood(obj, food);

                        if(food.target != null)
                            obj.GetComponent<NavMeshAgent>().SetDestination(food.target.transform.position);                   
                    }
                    else
                    {
                        //in vendetta state so now just want to kill is killer nothing else
                        if(!vendetta.target.GetECSComponent<Module.FollowTarget>().asBeenCalmDown)
                        {
                            obj.GetComponent<NavMeshAgent>().SetDestination(vendetta.target.transform.position);

                            if (Vector3.Distance(obj.transform.position, vendetta.target.transform.position) <= 1.2f)
                            {
                                Module.FollowTarget traget = vendetta.target.GetECSComponent<Module.FollowTarget>();
                                traget.asBeenCalmDown = true;
                                EntityActionBuffer.Instance.ApplyComponentChanges(vendetta.target, traget);
                            }

                        }
                        else
                        {
                            vendetta.wantToDoVendetta = false;
                            obj.GetComponent<MeshRenderer>().material.color = Color.green;
                            obj.GetComponent<TrailRenderer>().material.color = Color.green;
                            obj.GetComponent<NavMeshAgent>().speed = 4.5f;
                            EntityActionBuffer.Instance.ApplyComponentChanges(obj, vendetta);
                            
                        }
                    }
                });

            

        }

        private void EatFood(GameObject obj, Module.TargetEdible food)
        {
            if (food.target != null && Vector3.Distance(obj.transform.position, food.target.transform.position) <= 1.2f)
            {
                Module.Edible pocky = food.target.GetECSComponent<Module.Edible>();
                pocky.active = false;
                EntityActionBuffer.Instance.ApplyComponentChanges(food.target, pocky);

                Module.Score score = obj.GetECSComponent<Module.Score>();
                score.score += 1;
                EntityActionBuffer.Instance.ApplyComponentChanges(obj, score);
            }
        }

        private void Death(GameObject obj)
        {
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
        }

        private void pickClosestFood(GameObject obj, Module.TargetEdible food)
        {
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
        }

        private bool pickClosestEnemyInRange(GameObject obj, Module.Vendetta vendetta, float range)
        {
            float dist = range;
            new EntityQuery()
                .With<Module.FollowTarget>()
                .ForEach(enemy =>
                {
                    if (Vector3.Distance(obj.transform.position, enemy.transform.position) < dist)
                    {
                        vendetta.target = enemy;
                        dist = Vector3.Distance(obj.transform.position, enemy.transform.position);
                    }
                });
            EntityActionBuffer.Instance.ApplyComponentChanges<Module.Vendetta>(obj, vendetta);

            if (dist == range)
                return false;
            return true;
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
                    //check if food as been eaten
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
                Module.FollowTarget follow = new Module.FollowTarget(null,false);

                //entity
                GameObject tmp = ECS.EntityActionBuffer.Instance.CreateEntity(prefab);
                tmp.transform.position = GameMananger.RandomNavmeshLocation(400, tmp);

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
                    Module.FollowTarget follow = obj.GetECSComponent<Module.FollowTarget>();

                    //calming down the enemy
                    if (follow.asBeenCalmDown)
                    {
                        obj.transform.position = GameMananger.RandomNavmeshLocation(400f, obj);
                        obj.GetComponent<TrailRenderer>().Clear();

                        follow.asBeenCalmDown = false;

                        EntityActionBuffer.Instance.ApplyComponentChanges(obj, follow);
                    }

                    //we pick a new target
                    pickNewTarget(obj, follow);

                    //We see if the target is dead or not
                    KillTarget(obj, follow);

                    if(follow.target != null)
                        obj.GetComponent<NavMeshAgent>().SetDestination(follow.target.transform.position);

                });
        }

        private void pickNewTarget(GameObject obj, Module.FollowTarget follow)
        {
            //only pick new target once or if the new target want to do a Vendetta
            //it will chase it down until he is dead
            if (follow.target == null || follow.target.GetECSComponent<Module.Vendetta>().wantToDoVendetta)
            {
                float dist = float.MaxValue;
                new EntityQuery()
                .With<Module.Score>()
                .With<Module.Vendetta>()
                .ForEach(objToFollow =>
                {
                    //only pick green pecsman
                    Module.Vendetta vendetta = objToFollow.GetECSComponent<Module.Vendetta>();
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

        private void KillTarget(GameObject obj, Module.FollowTarget follow)
        {
            if (follow.target != null && Vector3.Distance(obj.transform.position, follow.target.transform.position) <= 1.0f)
            {
                EntityActionBuffer.Instance.ApplyComponentChanges(obj, follow);

                Module.Score score = follow.target.GetECSComponent<Module.Score>();
                score.isDead = true;
                EntityActionBuffer.Instance.ApplyComponentChanges(follow.target, score);

                //we set the target of the dead pecsman to the killer
                Module.Vendetta vendetta = follow.target.GetECSComponent<Module.Vendetta>();
                vendetta.wantToDoVendetta = true;
                vendetta.target = obj;

                follow.target.GetComponent<MeshRenderer>().material.color = Color.magenta;
                follow.target.GetComponent<TrailRenderer>().material.color = Color.magenta;
                follow.target.GetComponent<NavMeshAgent>().speed = 10.0f;

                EntityActionBuffer.Instance.ApplyComponentChanges(follow.target, vendetta);
            }
        }
    };
}
