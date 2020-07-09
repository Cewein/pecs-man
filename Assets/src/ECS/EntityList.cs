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

		public void InternalAdd(GameObject gameObject)
		{
			_entities.Add(gameObject);
		}

		public void InternalRemove(GameObject gameObject)
		{
			_entities.Remove(gameObject);
		}
	}
}