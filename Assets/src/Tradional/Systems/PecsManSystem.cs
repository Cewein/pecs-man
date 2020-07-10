using ECS;
using Module.Components;
using UnityEngine;
using UnityEngine.AI;

namespace Tradional.Systems
{
	public class PecsManSystem : ECSSystem
    {
        private GameObject _prefab;
        private int _number;

        public PecsManSystem(GameObject prefab, int number)
        {
            _prefab = prefab;
            _number = number;
        }

        private void Create()
        {
            for (int i = 0; i < _number; i++)
            {
                //component
                TargetEdible miam = new TargetEdible();
                Score score = new Score(i, 0, false);
                Vendetta vendetta = new Vendetta(false, null);

                //entity
                GameObject tmp = EntityActionBuffer.Instance.CreateEntity(_prefab);
                tmp.transform.position = GameMananger.RandomNavmeshLocation(40f, tmp);

                //merging both
                EntityActionBuffer.Instance.AddComponent(tmp, miam);
                EntityActionBuffer.Instance.AddComponent(tmp, score);
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
                .With<TargetEdible>()
                .With<Score>()
                .With<Vendetta>()
                .ForEach(obj =>
                {
                    Vendetta vendetta = obj.GetECSComponent<Vendetta>();

                    //pecsman death
                    Death(obj);

                    //normal behavior is not in vendetta state
                    if (!vendetta.WantToDoVendetta)
                    {
                        TargetEdible food = obj.GetECSComponent<TargetEdible>();

                        PickClosestFood(obj, food);

                        EatFood(obj, food);

                        if(food.Target != null)
                            obj.GetComponent<NavMeshAgent>().SetDestination(food.Target.transform.position);                   
                    }
                    else
                    {
                        //in vendetta state so now just want to kill is killer nothing else
                        if(!vendetta.Target.GetECSComponent<FollowTarget>().HasBeenCalmDown)
                        {
                            obj.GetComponent<NavMeshAgent>().SetDestination(vendetta.Target.transform.position);

                            if (Vector3.Distance(obj.transform.position, vendetta.Target.transform.position) <= 1.2f)
                            {
                                FollowTarget traget = vendetta.Target.GetECSComponent<FollowTarget>();
                                traget.HasBeenCalmDown = true;
                                EntityActionBuffer.Instance.ApplyComponentChanges(vendetta.Target, traget);
                            }

                        }
                        else
                        {
                            vendetta.WantToDoVendetta = false;
                            obj.GetComponent<MeshRenderer>().material.color = Color.green;
                            obj.GetComponent<TrailRenderer>().material.color = Color.green;
                            obj.GetComponent<NavMeshAgent>().speed = 4.5f;
                            EntityActionBuffer.Instance.ApplyComponentChanges(obj, vendetta);
                            
                        }
                    }
                });
        }

        private void EatFood(GameObject obj, TargetEdible food)
        {
            if (food.Target != null && Vector3.Distance(obj.transform.position, food.Target.transform.position) <= 1.2f)
            {
                Edible pocky = food.Target.GetECSComponent<Edible>();
                pocky.Active = false;
                EntityActionBuffer.Instance.ApplyComponentChanges(food.Target, pocky);

                Score score = obj.GetECSComponent<Score>();
                score.Value += 1;
                EntityActionBuffer.Instance.ApplyComponentChanges(obj, score);
            }
        }

        private void Death(GameObject obj)
        {
            if (obj.GetECSComponent<Score>().IsDead)
            {
                obj.transform.position = GameMananger.RandomNavmeshLocation(40f, obj);
                obj.GetComponent<TrailRenderer>().Clear();

                Score score = obj.GetECSComponent<Score>();

                MonoBehaviour.print($"pecsman N°{score.Id} with score of {score.Value}");

                score.Value = 0;
                score.IsDead = false;
                EntityActionBuffer.Instance.ApplyComponentChanges(obj, score);
            }
        }

        private void PickClosestFood(GameObject obj, TargetEdible food)
        {
            float dist = float.MaxValue;
            new EntityQuery()
                .With<Edible>()
                .ForEach(pocky =>
                {
                    if (Vector3.Distance(obj.transform.position, pocky.transform.position) < dist)
                    {
                        food.Target = pocky;
                        dist = Vector3.Distance(obj.transform.position, pocky.transform.position);
                    }
                });
            EntityActionBuffer.Instance.ApplyComponentChanges(obj, food);
        }
    }
}