using MARDEK.Battle;
using MARDEK.Stats;
using UnityEngine;

namespace MARDEK.Skill
{
	public abstract class ReactionSkill : Skill
	{
		[field: SerializeField] public float Strength { get; private set; }

		public abstract void Apply(ReactionModifiers modifiers);
	}

	// Shared behaviour for both offensive reaction types - only the concrete C# type differs
	// between the physical/magic variants below, so CharacterPlayable's per-category lists
	// (and the Unity object picker) can only ever accept the matching kind of skill.
	public abstract class OffensiveReactionSkill : ReactionSkill
	{
		[field: SerializeField] public OffensiveReactionType Type { get; private set; }

		public override void Apply(ReactionModifiers modifiers)
		{
			switch (Type)
			{
				case OffensiveReactionType.PercentDamageBonus:
					modifiers.DamageMultiplier *= 1f + Strength / 100f;
					break;
				case OffensiveReactionType.CritRateBonus:
					modifiers.CritRateBonus += Strength;
					break;
				case OffensiveReactionType.AccuracyBonus:
					modifiers.AccuracyMultiplier *= 1f + Strength / 100f;
					break;
			}
		}
	}

	// Shared behaviour for both defensive reaction types - same reasoning as
	// OffensiveReactionSkill above.
	public abstract class DefensiveReactionSkill : ReactionSkill
	{
		[field: SerializeField] public DefensiveReactionType Type { get; private set; }
		// Only used when Type == ElementalDamageReduction.
		[field: SerializeField] public Element Element { get; private set; }
		// Only used when Type == StatusResist.
		[field: SerializeField] public StatusEffect ResistedStatus { get; private set; }

		public override void Apply(ReactionModifiers modifiers)
		{
			switch (Type)
			{
				case DefensiveReactionType.FlatDamageReduction:
					modifiers.FlatDamageReduction += Strength;
					break;
				case DefensiveReactionType.PercentDamageReduction:
					modifiers.DamageMultiplier *= 1f - Strength / 100f;
					break;
				case DefensiveReactionType.ElementalDamageReduction:
					if (Element == null)
					{
						Debug.LogWarning($"{name}: ElementalDamageReduction has no Element assigned", this);
						break;
					}
					modifiers.ReduceElementalDamage(Element.ElementID, Strength / 100f);
					break;
				case DefensiveReactionType.StatusResist:
					modifiers.ResistedStatuses.Add(ResistedStatus);
					break;
				case DefensiveReactionType.EvasionBonus:
					modifiers.AccuracyMultiplier *= 1f - Strength / 100f;
					break;
			}
		}
	}

	public enum DefensiveReactionType
	{
		FlatDamageReduction,
		PercentDamageReduction,
		ElementalDamageReduction,
		StatusResist,
		EvasionBonus,
	}

	public enum OffensiveReactionType
	{
		PercentDamageBonus,
		CritRateBonus,
		AccuracyBonus,
	}
}
