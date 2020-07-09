using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace ECS
{
	public class ComponentListManager
	{
		private static ComponentListManager _instance;
		public static ComponentListManager Instance => _instance ?? (_instance = new ComponentListManager());

		private Dictionary<Type, Dictionary<GameObject, IComponent>> _componentLists = new Dictionary<Type, Dictionary<GameObject, IComponent>>();
		
		public bool HasComponent(GameObject gameObject, Type componentType)
		{
			return _componentLists[componentType].ContainsKey(gameObject);
		}
		
		public T Get<T>(GameObject gameObject)
			where T: struct, IComponent
		{
			return (T) Get(gameObject, typeof(T));
		}
		
		public IComponent Get(GameObject gameObject, Type type)
		{
			return _componentLists[type][gameObject];
		}
		
		public void InternalAdd(GameObject gameObject, IComponent component, Type type)
		{
			if (!_componentLists.ContainsKey(type))
			{
				_componentLists.Add(type, new Dictionary<GameObject, IComponent>());
			}
			_componentLists[type].Add(gameObject, component);
		}
		
		public void InternalRemove(GameObject gameObject, Type type)
		{
			_componentLists[type].Remove(gameObject);
		}

		public int GetComponentCount(Type componentType)
		{
			return _componentLists[componentType].Count;
		}

		public Dictionary<GameObject, IComponent>.KeyCollection GetEntitiesWithComponent(Type type)
		{
			return _componentLists[type].Keys;
		}
	}
}