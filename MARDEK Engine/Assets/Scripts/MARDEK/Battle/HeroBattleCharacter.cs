using System;
using MARDEK.Animation;
using MARDEK.CharacterSystem;
using UnityEngine;

namespace MARDEK.Battle
{
	public class HeroBattleCharacter : BattleCharacter
	{
		// TODO: flat for now - no level-scaling/formula yet.
		const int SkillUseExperience = 20;

		// exp = baseReward * base^(enemyLevel - killerLevel) - a level advantage/disadvantage
		// is worth a constant percentage per level regardless of how high the levels
		// involved are, unlike a raw level ratio (which flattens out at high levels: a
		// level 51 enemy vs a level 50 killer is only a 2% swing, the same +1 gap at level
		// 5 vs 6 is a 20% swing).
		const float LevelGapExponentBase = 1.1f;

		// The persistent CharacterPlayable this was spawned from - holds data that's
		// specific to being a hero (ReactionSkillset, PassiveSkillset) rather than something
		// every BattleCharacter needs.
		public CharacterPlayable Character { get; private set; }

		// Fired with the raw amount just added, regardless of whether that same grant also
		// triggered a level-up - PlayerCharacterUI.IncreaseExperience listens for this.
		public event Action<int> OnExperienceGained;
		// Fired after Level/Character.Level are updated - PlayerCharacterUI.IncreaseLevel
		// listens for this.
		public event Action OnLeveledUp;

		// Level-up is a side effect of the setter so every source of xp (kill, assist,
		// skill-use) gets it automatically. Excess xp beyond the threshold is discarded
		// rather than carried over, and only one level is gained per grant even if the
		// amount would cross more than one threshold - matches the spec literally; revisit
		// if a single huge reward should ever chain multiple level-ups.
		public int Experience
		{
			get => Character.Experience;
			private set
			{
				int gained = value - Character.Experience;
				Character.Experience = value;
				if (gained != 0)
					OnExperienceGained?.Invoke(gained);

				if (Character.Experience >= MaxExperience)
				{
					Character.Experience = 0;
					LevelUp();
				}
			}
		}

		public int MaxExperience => Level * 1000;

		void LevelUp()
		{
			Level++;
			Character.Level = Level;
			Debug.Log($"{Name} reached level {Level}!");
			OnLeveledUp?.Invoke();
		}

		// Called when this hero lands the killing blow on an enemy - they get the enemy's
		// ExperienceReward scaled by the level gap (or the flat skill-use amount, whichever
		// is bigger - see ActionSkill.TryPerformAction, which skips its own skill-use grant
		// on a kill so this doesn't double up), every other hero in the party gets half of
		// that same amount for free.
		public void GrantKillExperience(int baseReward, int enemyLevel)
		{
			int scaledKillReward = ScaleRewardByLevelGap(baseReward, enemyLevel - Level);
			int killReward = Mathf.Max(scaledKillReward, SkillUseExperience);

			Experience += killReward;
			int assistReward = killReward / 2;
			foreach (HeroBattleCharacter hero in BattleManager.PlayerBattleParty)
				if (hero != this)
					hero.Experience += assistReward;
		}

		static int ScaleRewardByLevelGap(int baseReward, int levelGap)
		{
			// Intentionally not clamped so you can't farm xp in low level areas and to make levelling up a new hero
			// to the same level as the rest of your party as quick and easy as possible.
			float multiplier = Mathf.Pow(LevelGapExponentBase, levelGap);
			return Mathf.RoundToInt(baseReward * multiplier);
		}

		// Called on every successful ActionSkill use (including basic Attack, which is just
		// an ActionSkill with Cost = 0) that doesn't kill anyone - not on failed attempts
		// (e.g. insufficient MP), not on items (which aren't ActionSkills), and not on a
		// kill (GrantKillExperience already takes the max against this same flat amount).
		public void GrantSkillUseExperience() => Experience += SkillUseExperience;

		public override void LoadCharacter(Character character)
		{
			Character = character as CharacterPlayable;
			if (Character == null)
				Debug.LogWarning($"{character.Profile.displayName} is used as a hero but isn't a CharacterPlayable - reaction/passive skillsets will be unavailable", character);

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

		public override Sprite[] GetBattleIcons()
		{
			SpriteAnimationClip clip = Profile.WalkSprites.GetClipByDirection(Vector2.down);
			return new Sprite[] { clip.GetSprite(0), clip.GetSprite(1) };
		}
	}
}
