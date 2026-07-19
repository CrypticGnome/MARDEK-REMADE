using System.Collections.Generic;
using MARDEK.Stats;

namespace MARDEK.Battle
{
	// Accumulates the effect of whichever reaction skills successfully fired during a single
	// action's resolution - populated by ReactionSkill.Apply (see MARDEK.Skill), read by
	// ActionEffects when the action is actually applied. A fresh instance is all-neutral, so
	// "no reaction happened" needs no special-casing downstream.
	public class ReactionModifiers
	{
		public float DamageMultiplier = 1f;
		public float FlatDamageReduction = 0f;
		public float AccuracyMultiplier = 1f;
		// Stored but not yet read anywhere - there's no crit-hit system yet.
		public float CritRateBonus = 0f;
		public HashSet<StatusEffect> ResistedStatuses = new();

		readonly Dictionary<ElementID, float> elementalDamageMultiplier = new();

		public void ReduceElementalDamage(ElementID element, float percent)
		{
			float current = elementalDamageMultiplier.TryGetValue(element, out float existing) ? existing : 1f;
			elementalDamageMultiplier[element] = current * (1f - percent);
		}

		public float GetElementalDamageMultiplier(ElementID element) =>
			elementalDamageMultiplier.TryGetValue(element, out float value) ? value : 1f;
	}
}
