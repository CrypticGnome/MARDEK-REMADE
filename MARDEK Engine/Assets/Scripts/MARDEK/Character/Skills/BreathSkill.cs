using UnityEngine;

namespace MARDEK.Skill
{
     using MARDEK.Battle;

     [CreateAssetMenu(menuName = "MARDEK/Skill/BreathSkill")]
     public class BreathSkill : ActionSkill
     {
          protected override ActionType ActionType => ActionType.Breath;
     }
}
