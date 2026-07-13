using UnityEngine;

namespace MARDEK.Skill
{
     using MARDEK.Battle;

     [CreateAssetMenu(menuName = "MARDEK/Skill/MeleeSkill")]
     public class MeleeSkill : ActionSkill
     {
          protected override ActionType ActionType => ActionType.Melee;
     }
}
