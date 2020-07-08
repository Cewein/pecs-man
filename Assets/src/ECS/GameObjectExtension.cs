﻿using UnityEngine;

namespace ECS
{
	public static class GameObjectExtension
	{
		public static bool HasECSComponent<T>(this GameObject gameObject) where T: IComponent, new()
		{
			return Accessor<T>.Instance.UsedBy(gameObject);
		}
		
		public static T GetECSComponent<T>(this GameObject gameObject) where T: IComponent, new()
		{
			return Accessor<T>.Instance.Get(gameObject);
		}
		
		public static T AddECSComponent<T>(this GameObject gameObject) where T: IComponent, new()
		{
			return Accessor<T>.Instance.Get(gameObject);
		}
	}
}