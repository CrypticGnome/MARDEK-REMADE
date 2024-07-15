using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.Skill
{
     using MARDEK.Battle;
     using MARDEK.CharacterSystem;
     using MARDEK.Core;
     using MARDEK.Stats;
     using System;

     [CreateAssetMenu(menuName = "MARDEK/Skill/ActionSkill")]
    public class ActionSkill : Skill, IAction
     {
          [SerializeField] List<EffectType> effects = new List<EffectType>();
          [field:SerializeField] public Element Element;
          public void Apply(Character user, Character target)
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
                         int STR = user.BaseStats.Strength;
                         int level = user.Level;
                         int damage = Math.Clamp(ATK - DEF, 0, int.MaxValue) * STR * (level + 5) / 50;
                         target.CurrentHP -= damage;
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