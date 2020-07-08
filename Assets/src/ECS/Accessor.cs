using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
	public class Accessor<T> where T: IComponent, new()
	{
		private static Accessor<T> _instance;
		public static Accessor<T> Instance => _instance ?? (_instance = new Accessor<T>());
		
		private Dictionary<GameObject, T> _components = new Dictionary<GameObject, T>();

		private Accessor()
		{
			
		}

		public bool UsedBy(GameObject gameObject)
		{
			return _components.ContainsKey(gameObject);
		}

		public T Get(GameObject gameObject)
		{
			return _components[gameObject];
		}

		public void Add(GameObject gameObject)
		{
			_components.Add(gameObject, new T());
		}
		
		public void Remove(GameObject gameObject)
		{
			_components.Remove(gameObject);
		}
	}
}