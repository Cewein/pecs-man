using ECS;
using UnityEngine;

namespace Module.Components
{
	public struct NavMeshAgent : IComponent
	{
		public UnityEngine.AI.NavMeshAgent UnityComponent;

		public NavMeshAgent(GameObject gameObject)
		{
			UnityComponent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
		}
	}
}