using ECS;
using UnityEngine;

namespace Module.Components
{
	public struct MeshRenderer : IComponent
	{
		public UnityEngine.MeshRenderer UnityComponent;

		public MeshRenderer(GameObject gameObject)
		{
			UnityComponent = gameObject.GetComponent<UnityEngine.MeshRenderer>();
		}
	}
}