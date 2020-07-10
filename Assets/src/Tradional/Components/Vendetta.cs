using ECS;
using UnityEngine;

namespace Module.Components
{
	public struct Vendetta : IComponent
	{
		public bool WantToDoVendetta;
		public GameObject Target;

		public Vendetta(bool wantToDoVendetta, GameObject target)
		{
			WantToDoVendetta = wantToDoVendetta;
			Target = target;
		}
	}
}