using System;
using MARDEK.Inventory;
using UnityEngine;
namespace MARDEK.CharacterSystem
{
	[CreateAssetMenu(fileName = "CharacterUnplayable", menuName = "Scriptable Objects/CharacterUnplayable")]
	public class CharacterUnplayable : Character
	{
		public ItemDrop[] Drops;

		// Defaults to the flat amount every kill used to grant, so existing enemies keep
		// their current behaviour until someone tunes them individually.
		[SerializeField] int experienceReward = 100;
		public int ExperienceReward => experienceReward;

		public override Character Clone(int level)
		{
			var clone = CreateInstance<CharacterUnplayable>();
			CopyBaseFieldsTo(clone, level);
			clone.Drops = Drops;
			clone.experienceReward = experienceReward;
			return clone;
		}

		[Serializable]
		public class ItemDrop
		{
			public Item Item;
			public float Chance;
		}
	}

}