using UnityEngine;

namespace ECS
{
	public static class GameObjectExtension
	{
		public static T GetECSComponent<T>(this GameObject gameObject)
			where T: struct, IComponent
		{
			return ComponentListManager.Instance.GetComponent<T>(gameObject);
		}
		
		public static void BufferedAddECSComponent<T>(this GameObject gameObject, T component)
			where T: struct, IComponent
		{
			EntityActionBuffer.Instance.AddComponent(gameObject, component);
		}
		
		public static void BufferedRemoveECSComponent<T>(this GameObject gameObject)
			where T: struct, IComponent
		{
			EntityActionBuffer.Instance.RemoveComponent<T>(gameObject);
		}
	}
}