namespace MARDEK.Battle
{
	using CharacterSystem;

	public class HeroBattleCharacter : BattleCharacter
	{
		public Character Character { get; private set; }
		public int Experience { get { return Character.Experience; } private set { Character.Experience = value; } }

		public override void LoadCharacter(Character character)
		{
			Character = character;
			Skillset = character.ActionSkillset;
			InitialiseFrom(character);

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
