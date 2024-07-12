using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.Skill
{
     using MARDEK.CharacterSystem;
     using MARDEK.Stats;
     using System;

     [CreateAssetMenu(menuName = "MARDEK/Skill/ActionSkill")]
    public class ActionSkill : Skill
    {
        [SerializeField] List<EffectType> effects = new List<EffectType>();

        public override void Apply(Character user, Character target)
        {
               foreach (var effect in effects)
                    ApplyEffect(user, target, effect);
        }

          public void ApplyEffect(Character user, Character target, EffectType effectType)
          {
               switch (effectType)
               {
                    default:
                         Debug.LogAssertion(effectType + " does not have an implemented effect");
                         break;
                    case EffectType.DealDamage:
                         int ATK = user.Attack;
                         int DEF = target.Defense;
                         int STR = user.BaseStats.CoreStats.Strength;
                         int level = user.BaseStats.Level;
                         int damage = Math.Clamp(ATK - DEF, 0, int.MaxValue) * STR * (level + 5) / 50;
                         target.BaseStats.CurrentHP -= damage;
                         break;

               }
          }
          public enum EffectType
          {
               DealDamage,
               Heal,
          }
     }
}