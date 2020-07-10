using ECS;
using UnityEngine;

namespace Module.Components
{
	public struct TargetEdible : IComponent
	{
		public GameObject Target;

		public TargetEdible(GameObject target)
		{
			Target = target;
		}
	}
}