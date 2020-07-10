using ECS;
using Module;
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
                    if (!vendetta.wantToDoVendetta)
                    {
                        TargetEdible food = obj.GetECSComponent<TargetEdible>();

                        PickClosestFood(obj, food);

                        EatFood(obj, food);

                        if(food.target != null)
                            obj.GetComponent<NavMeshAgent>().SetDestination(food.target.transform.position);                   
                    }
                    else
                    {
                        //in vendetta state so now just want to kill is killer nothing else
                        if(!vendetta.target.GetECSComponent<FollowTarget>().asBeenCalmDown)
                        {
                            obj.GetComponent<NavMeshAgent>().SetDestination(vendetta.target.transform.position);

                            if (Vector3.Distance(obj.transform.position, vendetta.target.transform.position) <= 1.2f)
                            {
                                FollowTarget traget = vendetta.target.GetECSComponent<FollowTarget>();
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

        private void EatFood(GameObject obj, TargetEdible food)
        {
            if (food.target != null && Vector3.Distance(obj.transform.position, food.target.transform.position) <= 1.2f)
            {
                Edible pocky = food.target.GetECSComponent<Edible>();
                pocky.active = false;
                EntityActionBuffer.Instance.ApplyComponentChanges(food.target, pocky);

                Score score = obj.GetECSComponent<Score>();
                score.score += 1;
                EntityActionBuffer.Instance.ApplyComponentChanges(obj, score);
            }
        }

        private void Death(GameObject obj)
        {
            if (obj.GetECSComponent<Score>().isDead)
            {
                obj.transform.position = GameMananger.RandomNavmeshLocation(40f, obj);
                obj.GetComponent<TrailRenderer>().Clear();

                Score score = obj.GetECSComponent<Score>();

                MonoBehaviour.print($"pecsman N°{score.number} with score of {score.score}");

                score.score = 0;
                score.isDead = false;
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
                        food.target = pocky;
                        dist = Vector3.Distance(obj.transform.position, pocky.transform.position);
                    }
                });
            EntityActionBuffer.Instance.ApplyComponentChanges(obj, food);
        }
    }
}