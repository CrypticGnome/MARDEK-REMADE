using UnityEngine;

namespace MARDEK.Skill
{
    using Core;
     using MARDEK.Stats;

     using System;
     public class SkillEffect : ScriptableObject
     {
          [SerializeField] EffectType effectType;
          // This is incredibly dumb. Ideally,this function would take 2 characters but the way the project is set up doesnt allow that
          public void Apply(IActor user, IStats target)
          {
                 switch (effectType) {
                      default:
                           Debug.LogAssertion(effectType + " does not have an implemented effect");
                           break;
                      case EffectType.DealDamage:
                         throw new NotImplementedException();
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