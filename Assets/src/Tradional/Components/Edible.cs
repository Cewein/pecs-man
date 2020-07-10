using ECS;

namespace Module.Components
{
	public struct Edible : IComponent
	{
		public bool Active;

		public Edible(bool active)
		{
			Active = active;
		}
	}
}