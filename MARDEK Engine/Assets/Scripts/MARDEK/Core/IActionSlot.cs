using MARDEK.CharacterSystem;
using UnityEngine;
namespace MARDEK.Core
{
	public interface IActionSlot
	{
		public string DisplayName { get; }
		public Sprite Sprite { get; }
		public int Number { get; }
		public string Description { get; }
		public void ApplyAction(Character user, Character target);
	}
}