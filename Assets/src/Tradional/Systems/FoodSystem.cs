using System;
using ECS;
using Module;
using UnityEngine;

namespace Tradional.Systems
{
	public class FoodSystem : ECSSystem
    {
        private GameObject _prefab;
        private int _number;

        public FoodSystem(GameObject prefab, int number)
        {
            _prefab = prefab;
            _number = number;
        }

        private void Create()
        {
            for (int i = 0; i < _number; i++)
            {
                //component
                Edible edible = new Edible();

                //entity
                GameObject tmp = EntityActionBuffer.Instance.CreateEntity(_prefab);
                tmp.transform.position = GameMananger.RandomNavmeshLocation(40, tmp);

                //merging both
                EntityActionBuffer.Instance.AddComponent(tmp, edible);
            }
        }

        public override void ExecuteOnce()
        {
            Create();
        }

        public override void Update()
        {
            new EntityQuery()
                .With<Edible>()
                .ForEach(obj =>
                {
                    //check if food as been eaten
                    Edible edible = obj.GetECSComponent<Edible>();
                    if (!edible.active)
                    {
                        obj.transform.position = GameMananger.RandomNavmeshLocation(40f, obj);
                        edible.active = true;
                        EntityActionBuffer.Instance.ApplyComponentChanges(obj, edible);
                    }
                });
        }
    }
}