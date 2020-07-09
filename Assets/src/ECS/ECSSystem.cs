using System.Collections.Generic;

namespace ECS
{
	public abstract class ECSSystem
	{
		private static List<ECSSystem> _systems = new List<ECSSystem>();

		public static void AddSystem(ECSSystem system)
		{
			_systems.Add(system);
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