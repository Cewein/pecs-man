using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
	public class EntityList
	{
		private static EntityList _instance;
		public static EntityList Instance => _instance ?? (_instance = new EntityList());

		private List<GameObject> _entities = new List<GameObject>();

		public int EntityCount => _entities.Count;

		public GameObject this[int i] => _entities[i];

		private EntityList()
		{
			
		}

		public GameObject Create()
		{
			GameObject gameObject = new GameObject();
			_entities.Add(gameObject);
			return gameObject;
		}
		
		public IEnumerator<GameObject> WithComponent<T1>()
			where T1: IComponent, new()
		{
			for (int i = 0; i < Instance.EntityCount; i++)
			{
				if (Instance[i].HasECSComponent<T1>())
				{
					yield return Instance[i];
				}
			}
		}
		
		public IEnumerator<GameObject> WithComponent<T1, T2>()
			where T1: IComponent, new()
			where T2: IComponent, new()
		{
			for (int i = 0; i < Instance.EntityCount; i++)
			{
				if (Instance[i].HasECSComponent<T1>() &&
				    Instance[i].HasECSComponent<T2>())
				{
					yield return Instance[i];
				}
			}
		}
		
		public IEnumerator<GameObject> WithComponent<T1, T2, T3>()
			where T1: IComponent, new()
			where T2: IComponent, new()
			where T3: IComponent, new()
		{
			for (int i = 0; i < Instance.EntityCount; i++)
			{
				if (Instance[i].HasECSComponent<T1>() &&
				    Instance[i].HasECSComponent<T2>() &&
				    Instance[i].HasECSComponent<T3>())
				{
					yield return Instance[i];
				}
			}
		}
	}
}