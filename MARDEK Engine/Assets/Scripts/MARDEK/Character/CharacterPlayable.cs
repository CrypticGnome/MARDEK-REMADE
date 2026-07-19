using System.Collections.Generic;
using MARDEK.Skill;
using UnityEngine;
namespace MARDEK.CharacterSystem
{
	[CreateAssetMenu(fileName = "CharacterPlayable", menuName = "MARDEK/Character/Playable Character")]

	public class CharacterPlayable : Character
	{
		[field: SerializeField] public List<PhysicalAttackReactionSkill> PhysicalAttackReactions { get; private set; }
		[field: SerializeField] public List<MagicAttackReactionSkill> MagicAttackReactions { get; private set; }
		[field: SerializeField] public List<PhysicalDefenseReactionSkill> PhysicalDefenseReactions { get; private set; }
		[field: SerializeField] public List<MagicDefenseReactionSkill> MagicDefenseReactions { get; private set; }
		[field: SerializeField] public PassiveSkillset PassiveSkillset { get; private set; }
	}
}