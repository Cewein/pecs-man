using ECS;
using UnityEngine;

namespace Module.Components
{
	public struct TrailRenderer : IComponent
	{
		public UnityEngine.TrailRenderer UnityComponent;

		public TrailRenderer(GameObject gameObject)
		{
			UnityComponent = gameObject.GetComponent<UnityEngine.TrailRenderer>();
		}
	}
}