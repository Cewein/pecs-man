using System;
using System.Collections.Generic;
using System.Reflection;

namespace ECS
{
	public abstract class ECSSystem
	{
		private static List<ECSSystem> _systems = new List<ECSSystem>();

		public static void ScanForEnabledSystems()
		{
			foreach(Type type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (type.GetCustomAttributes(typeof(EnableECSSystemAttribute), false).Length > 0)
				{
					if (!type.IsSubclassOf(typeof(ECSSystem)))
					{
						throw new InvalidOperationException($"Class {type} does not inherits from ECSSystem");
					}
					_systems.Add((ECSSystem)Activator.CreateInstance(type));
				}
			}
		}
		
		public static void StartAll()
		{
			for (int i = 0; i < _systems.Count; i++)
			{
				_systems[i].ExecuteOnce();
			}
		}

		public static void UpdateAll()
		{
			for (int i = 0; i < _systems.Count; i++)
			{
				_systems[i].Update();
			}
		}

		public virtual void Update(){}
		public virtual void ExecuteOnce(){}
	}
}