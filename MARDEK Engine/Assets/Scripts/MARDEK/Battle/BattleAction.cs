using System.Collections.Generic;
using UnityEngine;
using System;
using MARDEK.Audio;
using MARDEK.Stats;

namespace MARDEK.Battle
{

	[Serializable]
	public class BattleAction
	{
		[SerializeField] Element element;
		[SerializeField] SoundEffect[] soundEffects;
		[SerializeReference, SubclassSelector] ActionEffects[] actionEffects;
		[SerializeField] TargetScope targetScope = TargetScope.SingleOnly;

		[HideInInspector] public ActionType ActionType;
		public Element Element { get { return element; } }
		public TargetScope TargetScope => targetScope;

		// Used by BattleCharacterPicker to default target selection to the caster's own
		// team for heals rather than the enemy team.
		public bool TargetsAllies => Array.Exists(actionEffects, effect =>
			effect is DefaultHeal or ManaHeal or ConstHeal);

		// Used by BattleCharacterPicker/EnemyBattleCharacter to decide which pair of
		// reaction lists (physical vs magic) are eligible for this action. Neither is true
		// for actions with no damage effect (heals, buffs, status-only) - those don't get a
		// reaction window at all.
		public bool IsPhysicalAttack => Array.Exists(actionEffects, effect => effect is DealMeleeDamageStandard);
		public bool IsMagicAttack => Array.Exists(actionEffects, effect => effect is DealMagicDamageStandard);

		// efficacyMultiplier scales every effect applied to every target - used to halve
		// per-target output when a SingleOrAll/AllOnly action is used against everyone on a
		// side instead of a single target. reactionModifiers carries whatever a successful
		// reaction-bar hit contributed (see ReactionModifiers) - defaults to a fresh, neutral
		// instance so callers that don't care about reactions (e.g. items) don't need one.
		// Sound plays once regardless of target count.
		public void Apply(BattleCharacter user, IReadOnlyList<BattleCharacter> targets, float efficacyMultiplier = 1f, ReactionModifiers reactionModifiers = null)
		{
			reactionModifiers ??= new ReactionModifiers();
			foreach (BattleCharacter target in targets)
				for (int index = 0; index < actionEffects.Length; index++)
					actionEffects[index].ApplyEffect(user, target, element, efficacyMultiplier, reactionModifiers);
			AudioManager.PlayEffectString(soundEffects);
		}
	}

	public enum ActionType
	{
		Melee,
		Spellcast,
		Breath,
		Item,
	}

	// SingleOnly: always one target, no toggle offered.
	// SingleOrAll: player can toggle between one target and everyone on a side (this pass).
	// AllOnly: always hits everyone on a side, no single-target fallback.
	// Multiselect: reserved for choosing an arbitrary subset of targets - not implemented yet.
	public enum TargetScope
	{
		SingleOnly,
		SingleOrAll,
		AllOnly,
		Multiselect,
	}
}
