using ECS;
using Module.Components;
using UnityEngine;
using MeshRenderer = Module.Components.MeshRenderer;
using TrailRenderer = Module.Components.TrailRenderer;
using NavMeshAgent = Module.Components.NavMeshAgent;

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
                //entity
                GameObject tmp = EntityActionBuffer.Instance.CreateEntity(_prefab);
                tmp.transform.position = GameMananger.RandomNavmeshLocation(40f, tmp);
                
                //component
                TargetEdible miam = new TargetEdible();
                Score score = new Score(i, 0, false);
                Vendetta vendetta = new Vendetta(false, null);
                TrailRenderer trailRenderer = new TrailRenderer(tmp);
                MeshRenderer meshRenderer = new MeshRenderer(tmp);
                NavMeshAgent navMeshAgent = new NavMeshAgent(tmp);

                //merging both
                EntityActionBuffer.Instance.AddComponent(tmp, miam);
                EntityActionBuffer.Instance.AddComponent(tmp, score);
                EntityActionBuffer.Instance.AddComponent(tmp, vendetta);
                EntityActionBuffer.Instance.AddComponent(tmp, trailRenderer);
                EntityActionBuffer.Instance.AddComponent(tmp, meshRenderer);
                EntityActionBuffer.Instance.AddComponent(tmp, navMeshAgent);
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
                            obj.GetECSComponent<NavMeshAgent>().UnityComponent.SetDestination(food.Target.transform.position);                   
                    }
                    else
                    {
                        //in vendetta state so now just want to kill is killer nothing else
                        if(!vendetta.Target.GetECSComponent<FollowTarget>().HasBeenCalmDown)
                        {
                            obj.GetECSComponent<NavMeshAgent>().UnityComponent.SetDestination(vendetta.Target.transform.position);

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
                            obj.GetECSComponent<MeshRenderer>().UnityComponent.material.color = Color.green;
                            obj.GetECSComponent<TrailRenderer>().UnityComponent.material.color = Color.green;
                            obj.GetECSComponent<NavMeshAgent>().UnityComponent.speed = 4.5f;
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
                obj.GetECSComponent<TrailRenderer>().UnityComponent.Clear();

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