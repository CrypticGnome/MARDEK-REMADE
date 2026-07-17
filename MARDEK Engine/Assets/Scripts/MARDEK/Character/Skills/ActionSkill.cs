using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.Skill
{
	using MARDEK.Battle;

	public abstract class ActionSkill : Skill, IBattleAction
	{
		[field: SerializeField] public int Cost { get; private set; }
		[SerializeField] BattleAction action;
		public BattleAction Action { get { return action; } }

		public Sprite ActionIcon => Action.Element.thickSprite;

		protected abstract ActionType ActionType { get; }

		void OnEnable()
		{
			action.ActionType = ActionType;
		}

		// Hitting every target on a side costs double and halves each target's efficacy -
		// determined purely by target count, so a SingleOrAll/AllOnly action that only has
		// one eligible target left is charged the normal single-target price.
		public bool TryPerformAction(BattleCharacter user, IReadOnlyList<BattleCharacter> targets)
		{
			bool targetingAll = targets.Count > 1;
			int cost = targetingAll ? Cost * 2 : Cost;
			if (user.CurrentMP < cost) return false;

			action.Apply(user, targets, targetingAll ? 0.5f : 1f);
			user.CurrentMP -= cost;
			return true;
		}
	}
}