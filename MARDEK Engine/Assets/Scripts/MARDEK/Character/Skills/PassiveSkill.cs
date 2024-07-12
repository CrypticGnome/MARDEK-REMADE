using UnityEngine;

namespace MARDEK.Skill
{
     using MARDEK.CharacterSystem;

     [CreateAssetMenu(menuName = "MARDEK/Skill/PassiveSkill")]
    public class PassiveSkill : Skill
    {
        public override void Apply(Character user, Character target)
        {
            throw new System.NotImplementedException();
        }
    }
}