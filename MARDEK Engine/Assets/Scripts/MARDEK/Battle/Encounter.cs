using MARDEK.CharacterSystem;
using MARDEK.Inventory;
using UnityEngine;

namespace MARDEK.Battle
{
	[CreateAssetMenu(menuName = "MARDEK/Battle/Encounter")]
	public class Encounter : ScriptableObject
	{
		public EncounterType Type = EncounterType.Standard;
		public EnemyWithLevelRange[] Enemies;
		public Item[] UniqueRewards;

		[System.Serializable]
		public class EnemyWithLevelRange
		{
			public Character enemy;
			public int minLevel = 0;
			public int maxLevel = 0;
		}
	}

	public enum EncounterType
	{
		Standard,
		Grand,
	}
}