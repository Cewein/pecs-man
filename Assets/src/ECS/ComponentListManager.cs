using System;
using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
	public class ComponentListManager
	{
		private static ComponentListManager _instance;
		public static ComponentListManager Instance => _instance ?? (_instance = new ComponentListManager());

		private Dictionary<Type, Dictionary<GameObject, IComponent>> _componentLists = new Dictionary<Type, Dictionary<GameObject, IComponent>>();

		private void EnsureComponentListExists(Type componentType)
		{
			if (!_componentLists.ContainsKey(componentType))
			{
				_componentLists.Add(componentType, new Dictionary<GameObject, IComponent>());
			}
		}
		
		public bool HasComponent(GameObject gameObject, Type componentType)
		{
			EnsureComponentListExists(componentType);
			return _componentLists[componentType].ContainsKey(gameObject);
		}
		
		public T Get<T>(GameObject gameObject)
			where T: struct, IComponent
		{
			EnsureComponentListExists(typeof(T));
			return (T) Get(gameObject, typeof(T));
		}
		
		public IComponent Get(GameObject gameObject, Type componentType)
		{
			EnsureComponentListExists(componentType);
			return _componentLists[componentType][gameObject];
		}
		
		public void InternalAdd(GameObject gameObject, IComponent component, Type componentType)
		{
			EnsureComponentListExists(componentType);
			_componentLists[componentType].Add(gameObject, component);
		}
		
		public void InternalRemove(GameObject gameObject, Type componentType)
		{
			EnsureComponentListExists(componentType);
			_componentLists[componentType].Remove(gameObject);
		}

		public int GetComponentCount(Type componentType)
		{
			EnsureComponentListExists(componentType);
			return _componentLists[componentType].Count;
		}

		public Dictionary<GameObject, IComponent>.KeyCollection GetEntitiesWithComponent(Type componentType)
		{
			EnsureComponentListExists(componentType);
			return _componentLists[componentType].Keys;
		}
	}
}