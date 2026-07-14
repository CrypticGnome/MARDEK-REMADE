using UnityEngine;

namespace MARDEK.Skill
{
	using MARDEK.Battle;

	[CreateAssetMenu(menuName = "MARDEK/Skill/SpellSkill")]
	public class SpellSkill : ActionSkill
	{
		protected override ActionType ActionType => ActionType.Spellcast;
	}
}
