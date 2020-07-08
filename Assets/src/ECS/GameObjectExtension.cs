using UnityEngine;

namespace ECS
{
	public static class GameObjectExtension
	{
		public static bool HasECSComponent<T>(this GameObject gameObject) where T: IComponent, new()
		{
			return ComponentListManager.Instance.UsedBy<T>(gameObject);
		}
		
		public static T GetECSComponent<T>(this GameObject gameObject) where T: IComponent, new()
		{
			return ComponentListManager.Instance.Get<T>(gameObject);
		}
		
		public static void BufferedAddECSComponent<T>(this GameObject gameObject, T component) where T: IComponent, new()
		{
			EntityActionBuffer.Instance.AddComponent(gameObject, component);
		}
		
		public static void BufferedRemoveECSComponent<T>(this GameObject gameObject) where T: IComponent, new()
		{
			EntityActionBuffer.Instance.RemoveComponent<T>(gameObject);
		}
	}
}