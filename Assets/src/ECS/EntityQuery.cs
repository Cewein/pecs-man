using System;
using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
	public class EntityQuery
	{
		private List<Type> _neededComponents = new List<Type>();
		
		public EntityQuery With<T>()
		{
			_neededComponents.Add(typeof(T));
			return this;
		}

		public void ForEach(Action<GameObject> action)
		{
			if (_neededComponents.Count < 1)
			{
				throw new InvalidOperationException("Cannot make an entity query without any component constraint");
			}
			
			//We check every components to retrieve the one with the least instances. This will allows us to possibly iterate over fewer entities.
			int lowestSize = ComponentListManager.Instance.GetComponentCount(_neededComponents[0]);
			int lowestIndex = 0;
			for (int i = 1; i < _neededComponents.Count; i++)
			{
				int currSize = ComponentListManager.Instance.GetComponentCount(_neededComponents[i]);
				if (currSize < lowestSize)
				{
					lowestIndex = i;
				}
			}
			Type componentTypeWithLeastInstances = _neededComponents[lowestIndex];
			
			//We retrieve the entities with the component type retrieved above
			Dictionary<GameObject, IComponent>.KeyCollection entitiesWithComponent = ComponentListManager.Instance.GetEntitiesWithComponent(componentTypeWithLeastInstances);
			foreach (GameObject entity in entitiesWithComponent)
			{
				//We check if the entities also have other needed components
				bool entityIsValid = true;
				foreach (Type type in _neededComponents)
				{
					if (type != componentTypeWithLeastInstances && !ComponentListManager.Instance.HasComponent(entity, type))
					{
						entityIsValid = false;
						break;
					}
				}

				//If the entity corresponds to the required components
				if (entityIsValid)
				{
					action.Invoke(entity);
				}
			}
		}
	}
}