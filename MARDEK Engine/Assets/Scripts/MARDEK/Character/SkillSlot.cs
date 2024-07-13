using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.CharacterSystem
{
    using Core;
    using Stats;
    [System.Serializable]
    public class SkillSlot : IActionSlot
    {
        [field: SerializeField] public Skill.Skill Skill {get;set;}


        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(Skill.DisplayName))
                    return Skill.name;
                else
                    return Skill.DisplayName;
            }
        }
        public Sprite Sprite
        {
            get
            {
                return Skill.Element.thickSprite;
            }
        }
        public int Number => Skill.Cost;
        public string Description => Skill.Description;

        public void ApplyAction(Character user, Character target)
        {
            Debug.Log($"Used {Skill.name}");
            Skill.Apply(user, target);
        }
    }
}