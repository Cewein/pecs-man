using System;
using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
	public class ComponentListManager
	{
		private static ComponentListManager _instance;
		public static ComponentListManager Instance => _instance ?? (_instance = new ComponentListManager());

		private Dictionary<Type, ComponentList> _componentLists = new Dictionary<Type, ComponentList>();

		public bool InternalUsedBy(GameObject gameObject, Type type)
		{
			return _componentLists[type].UsedBy(gameObject);
		}
		
		public T Get<T>(GameObject gameObject)
			where T: struct, IComponent
		{
			return (T) Get(gameObject, typeof(T));
		}
		
		public IComponent Get(GameObject gameObject, Type type)
		{
			return _componentLists[type].GetComponent(gameObject);
		}
		
		public void InternalAdd(GameObject gameObject, IComponent component, Type type)
		{
			if (!_componentLists.ContainsKey(type))
			{
				_componentLists.Add(type, new ComponentList());
			}
			_componentLists[type].AddComponent(gameObject, component);
		}
		
		public void InternalRemove(GameObject gameObject, Type type)
		{
			_componentLists[type].RemoveComponent(gameObject);
		}
		
		private class ComponentList
		{
			private Dictionary<GameObject, IComponent> _components = new Dictionary<GameObject, IComponent>();

			public bool UsedBy(GameObject gameObject)
			{
				return _components.ContainsKey(gameObject);
			}

			public IComponent GetComponent(GameObject gameObject)
			{
				return _components[gameObject];
			}

			public void AddComponent(GameObject gameObject, IComponent component)
			{
				_components.Add(gameObject, component);
			}
		
			public void RemoveComponent(GameObject gameObject)
			{
				_components.Remove(gameObject);
			}
		}
	}
}