using System;

namespace MARDEK.Core.LevelDesign
{
	[Serializable]
	public abstract class Condition
	{
		public abstract bool Value { get; }
	}
}