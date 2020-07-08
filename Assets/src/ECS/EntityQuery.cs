using System;
using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
	public class EntityQuery
	{
		private List<Type> _neededComponents = new List<Type>();
		
		public EntityQuery WithComponent<T>()
		{
			_neededComponents.Add(typeof(T));
			return this;
		}

		public void ForEach(Action<GameObject> action)
		{
			EntityList list = EntityList.Instance;
			for (int i = 0; i < list.EntityCount; i++)
			{
				bool gameObjectIsValid = true;
				for (int j = 0; j < _neededComponents.Count; j++)
				{
					if (!ComponentListManager.Instance.InternalUsedBy(list[i], _neededComponents[j]))
					{
						gameObjectIsValid = false;
						break;
					}
				}

				if (gameObjectIsValid)
				{
					action.Invoke(list[i]);
				}
			}
		}
	}
}