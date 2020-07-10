using ECS;

namespace Module.Components
{
	public struct Score : IComponent
	{
		public int Value;
		public bool IsDead;
		public int Id;

		public Score(int id, int value, bool isDead)
		{
			Id = id;
			Value = value;
			IsDead = isDead;
		}
	}
}