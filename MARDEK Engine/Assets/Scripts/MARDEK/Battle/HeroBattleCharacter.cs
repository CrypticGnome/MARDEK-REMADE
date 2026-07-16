using UnityEngine;

namespace MARDEK.Battle
{
	using CharacterSystem;
	using MARDEK.Stats;

	public class HeroBattleCharacter : BattleCharacter
	{
		public Character Character { get; private set; }
		public int Experience { get { return Character.Experience; } private set { Character.Experience = value; } }
		public HeroBattleCharacter(Character character, Transform parent)
		{
			Character = character;
			Level = character.Level;
			Profile = character.Profile;
			Skillset = character.ActionSkillset;
			GameObject prefabInstance = Object.Instantiate(Profile.BattleModelPrefab, parent);
			battleModel = prefabInstance.GetComponent<BattleModelComponent>();


			VolatileStats = new CoreStats(BaseStats);
			BaseStats.CalculateMaxValues(this);
			VolatileStats.CalculateMaxValues(this);

			CurrentHP = character.CurrentHP;
			CurrentMP = character.CurrentMP;

			VolatileStats.Attack = character.Attack;
			VolatileStats.Defense = character.Defense;
			VolatileStats.MagicDefense = character.MagicDefense;
		}

		// Writes battle HP/MP back to the persistent Character this was built from.
		// Uses the held reference rather than list-index matching, so it stays correct
		// even if PlayerBattleParty and the persistent party ever diverge in order/length.
		public void SyncToCharacter()
		{
			Character.CurrentHP = CurrentHP;
			Character.CurrentMP = CurrentMP;
		}
	}
}