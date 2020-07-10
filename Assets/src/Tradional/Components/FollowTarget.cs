using ECS;
using UnityEngine;

namespace Module.Components
{
	public struct FollowTarget : IComponent
	{
		public GameObject Target;
		public bool HasBeenCalmDown;

		public FollowTarget(GameObject target, bool hasBeenCalmDown)
		{
			Target = target;
			HasBeenCalmDown = hasBeenCalmDown;
		}
	}
}