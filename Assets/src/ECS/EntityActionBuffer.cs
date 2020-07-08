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

		private Dictionary<GameObject, IComponent> _addedComponentList = new Dictionary<GameObject, IComponent>();
		private Dictionary<GameObject, Type> _removedComponentList = new Dictionary<GameObject, Type>();

		public GameObject CreateEntity()
		{
			GameObject gameObject = new GameObject();
			_createdEntityList.Add(gameObject);
			return gameObject;
		}
		
		public void RemoveEntity(GameObject gameObject)
		{
			_removedEntityList.Add(gameObject);
		}
		
		public void AddComponent<T>(GameObject gameObject, T component)
			where T: IComponent, new()
		{
			_addedComponentList.Add(gameObject, component);
		}
		
		public void RemoveComponent<T>(GameObject gameObject)
			where T: IComponent, new()
		{
			_removedComponentList.Add(gameObject, typeof(T));
		}
		
		// ReSharper disable ForCanBeConvertedToForeach
		public void FlushBufferedActions()
		{
			for (int i = 0; i < _createdEntityList.Count; i++)
			{
				EntityList.Instance.Add(_createdEntityList[i]);
			}
			_createdEntityList.Clear();
			
			for (int i = 0; i < _removedEntityList.Count; i++)
			{
				EntityList.Instance.Remove(_removedEntityList[i]);
			}
			_removedEntityList.Clear();

			foreach (KeyValuePair<GameObject,IComponent> pair in _addedComponentList)
			{
				ComponentListManager.Instance.InternalAdd(pair.Key, pair.Value, pair.Value.GetType());
			}
			_createdEntityList.Clear();
			
			foreach (KeyValuePair<GameObject, Type> pair in _removedComponentList)
			{
				ComponentListManager.Instance.InternalRemove(pair.Key, pair.Value);
			}
			_removedEntityList.Clear();
		}
	}
}