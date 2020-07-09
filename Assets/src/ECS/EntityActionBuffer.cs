using System;
using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
	public class EntityActionBuffer
	{
		private static EntityActionBuffer _instance;
		public static EntityActionBuffer Instance => _instance ?? (_instance = new EntityActionBuffer());

		private List<GameObject> _createdEntityList = new List<GameObject>();
		private List<GameObject> _removedEntityList = new List<GameObject>();

		private Dictionary<GameObject, HashSet<IComponent>> _addedComponentList = new Dictionary<GameObject, HashSet<IComponent>>();
		private Dictionary<GameObject, HashSet<Type>> _removedComponentList = new Dictionary<GameObject, HashSet<Type>>();

		public GameObject CreateEntity()
		{
			GameObject gameObject = new GameObject();
			_createdEntityList.Add(gameObject);
			return gameObject;
		}

        public GameObject CreateEntity(GameObject prefab)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
            _createdEntityList.Add(gameObject);
            return gameObject;
        }

        public void RemoveEntity(GameObject gameObject)
		{
			_removedEntityList.Add(gameObject);
		}
		
		public void AddComponent<T>(GameObject gameObject, T component)
			where T: struct, IComponent
		{
            if(!_addedComponentList.ContainsKey(gameObject))
                _addedComponentList.Add(gameObject, new HashSet<IComponent>());

            _addedComponentList[gameObject].Add(component);
		}
		
		public void RemoveComponent<T>(GameObject gameObject)
			where T: struct, IComponent
		{
            if (!_removedComponentList.ContainsKey(gameObject))
                _removedComponentList.Add(gameObject, new HashSet<Type>());

            _removedComponentList[gameObject].Add(typeof(T));
		}
		
		// ReSharper disable ForCanBeConvertedToForeach
		public void FlushBufferedActions()
		{
			for (int i = 0; i < _createdEntityList.Count; i++)
			{
				EntityList.Instance.InternalAdd(_createdEntityList[i]);
			}
			_createdEntityList.Clear();
			
			for (int i = 0; i < _removedEntityList.Count; i++)
			{
				EntityList.Instance.InternalRemove(_removedEntityList[i]);
			}
			_removedEntityList.Clear();

			foreach (KeyValuePair<GameObject, HashSet<IComponent>> pair in _addedComponentList)
			{
                foreach (IComponent item in pair.Value)
                {
				    ComponentListManager.Instance.InternalAdd(pair.Key, item);
                }
			}
            _addedComponentList.Clear();
			
			foreach (KeyValuePair<GameObject, HashSet<Type>> pair in _removedComponentList)
			{
                foreach (Type item in pair.Value) 
                {
				    ComponentListManager.Instance.InternalRemove(pair.Key, item);
                }
			}
            _removedComponentList.Clear();
		}
	}
}