using System.Collections.Generic;
using MARDEK.Battle;
using UnityEngine;

namespace MARDEK.Inventory
{
	[CreateAssetMenu(menuName = "MARDEK/Inventory/ExpendableItem")]
	public class ExpendableItem : Item, IBattleAction
	{
		[SerializeField] string _colorHexCode;
		[SerializeField] string _pfx;

		[SerializeField] BattleAction action;
		public BattleAction Action { get { return action; } }

		public Sprite ActionIcon => sprite;

		public string DisplayName => displayName;

		public ExpendableItem()
		{
			//statsSet = new StatsSet();
		}

		override public Color GetInventorySpaceColor()
		{
			return new Color(81f / 255f, 113f / 255f, 217f / 255f);
		}

		public bool TryPerformAction(BattleCharacter user, IReadOnlyList<BattleCharacter> targets, ReactionModifiers reactionModifiers)
		{
			// Items are excluded from the reaction system - reactionModifiers is always
			// neutral here, but still threaded through for a uniform ApplyEffect signature.
			action.Apply(user, targets, 1f, reactionModifiers);
			return true;
		}
	}
}
