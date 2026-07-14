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

		public bool TryPerformAction(BattleCharacter user, BattleCharacter target)
		{
			if (user.CurrentMP < Cost) return false;
			action.Apply(user, target);
			user.CurrentMP -= Cost;
			return true;
		}
	}
}