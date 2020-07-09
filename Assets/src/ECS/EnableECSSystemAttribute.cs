using System;
using System.Runtime.InteropServices;

namespace ECS
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class EnableECSSystemAttribute : Attribute
	{
		
	}
}