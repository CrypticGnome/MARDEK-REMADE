
using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.Battle
{
	using System.Collections;
	using CharacterSystem;
	using MARDEK.Skill;

	public class EnemyBattleCharacter : BattleCharacter
	{
		public ActionSkill NextAction { get; private set; }
		public BattleCharacter NextTarget { get; private set; }

		// The persistent CharacterUnplayable this was spawned from - holds data that's
		// specific to being an enemy (loot Drops, ExperienceReward) rather than something
		// every BattleCharacter needs.
		public CharacterUnplayable Character { get; private set; }
		public int ExperienceReward => Character != null ? Character.ExperienceReward : 0;

		public override void LoadCharacter(Character character)
		{
			Character = character as CharacterUnplayable;
			if (Character == null)
				Debug.LogWarning($"{character.Profile.displayName} is used as an enemy but isn't a CharacterUnplayable - loot drops and experience reward will be unavailable", character);

			InitialiseFrom(character);
			Skillset = Profile.LearnableSkillset;

			CurrentHP = VolatileStats.MaxHP;
			CurrentMP = VolatileStats.MaxMP;
		}

		// Picks the skill/target this enemy will use on its next turn, so there's always
		// a queued attack available to show in the UI ahead of time.
		public void ChooseNextAction(IReadOnlyList<BattleCharacter> possibleTargets)
		{
			if (Skillset is null || Skillset.Skills.Count == 0)
			{
				Debug.LogWarning($"{Name}'s moveset is null or empty");
				NextAction = null;
				NextTarget = null;
				return;
			}
			ActionSkill skill = Skillset.Skills[Random.Range(0, Skillset.Skills.Count)];
			if (skill is null)
			{
				Debug.LogError($"{Name}'s moveset '{Skillset.name}' has an unassigned skill slot", Skillset);
				NextAction = null;
				NextTarget = null;
				return;
			}

			NextAction = skill;
			NextTarget = PickTarget(possibleTargets);

			Debug.Log(NextTarget != null
				? $"{Name} prepares to use {skill.DisplayName} on {NextTarget.Name}"
				: $"{Name} prepares to use {skill.DisplayName} but has no valid target");
		}

		static BattleCharacter PickTarget(IReadOnlyList<BattleCharacter> possibleTargets)
		{
			List<BattleCharacter> aliveTargets = new List<BattleCharacter>();
			foreach (BattleCharacter target in possibleTargets)
				if (target.CurrentHP > 0) aliveTargets.Add(target);

			if (aliveTargets.Count == 0) return null;
			return aliveTargets[Random.Range(0, aliveTargets.Count)];
		}

		public override IEnumerator TakeAction()
		{
			BattleCharacter target = ResolveTarget();
			if (NextAction is null || target is null)
			{
				BattleManager.EndCurrentTurn();
				yield break;
			}

			Debug.Log($"{Name} uses {NextAction.DisplayName}");
			BattleManager.PerformActionToTarget(NextAction, target);
			BattleManager.DisplayAction(NextAction);

			// Immediately queue up this enemy's following attack so one is always ready to show ahead of time.
			ChooseNextAction(BattleManager.PlayerBattleParty);
		}

		// The queued target may have died since it was chosen, so fall back to a fresh pick.
		BattleCharacter ResolveTarget() =>
			NextTarget != null && NextTarget.CurrentHP > 0 ? NextTarget : PickTarget(BattleManager.PlayerBattleParty);

		public override IEnumerator Die()
		{
			yield return battleModel.PlayDeathSequence();

			// Hide only the visuals child - this GameObject stays active, so the dead enemy
			// remains in EnemyBattleParty as a valid object (turn order, target selection and
			// the victory check all filter on IsDead instead).
			battleModel.SetModelActive(false);
		}
	}
}
