using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MARDEK.Battle
{
     using MARDEK.CharacterSystem;
     using MARDEK.Stats;
     using System;
     [Serializable]
     public class Action
     {
          [SerializeField] List<Effect> effects = new List<Effect>();
          [SerializeField] Element element;
          public Element Element { get { return element; } }
          public void Apply(Character user, Character target)
          {
               for (int index = 0; index < effects.Count; index++) 
                    ApplyEffect(user, target, effects[index]);
          }

          void ApplyEffect(Character user, Character target, Effect effect)
          {
               float ATK = user.Attack;
               float DEF = target.Defense;
               float MDEF = target.MagicDefense;
               float STR = user.BaseStats.Strength;
               float SPR = user.BaseStats.Spirit;
               float level = user.Level;
               EffectType effectType = effect.EffectType;
               float motionValue = effect.MotionValue;
               int elementalAbsorbtion = target.Absorbtions.GetAbsorbtion(element.ElementID);
               float elementalVulnerability = 1 - (float)elementalAbsorbtion / 100;

               switch (effectType)
               {
                    default:
                         Debug.LogAssertion(effectType + " does not have an implemented effect");
                         return;
                    case EffectType.MeleeDamage:

                         ///// Helps smooth out the awkwardness of def being too weak at low values and too strong at high values (default: Clamp0(ATK - DEF))
                         //
                         ///// Basing linearly of strength is not ideal. However, with strength raising potions in the game having non-linear scaling would be dangerous
                         //float power = STR * LevelDamageMultiplier(level); 
                         if (element is null)
                         {
                              Debug.LogAssertion("Element cannot be null in a damage effect");
                              return;
                         }
                         float finalAttack = motionValue * ATK * elementalVulnerability;
                         //float rawDamage = Clamp0(finalAttack - DEF);
                         float rawDamage = Clamp0(finalAttack - DEF / 2) * finalAttack / (finalAttack + DEF);
                         float power = STR * (level + 5) / 50;
                         int damage = (int)(rawDamage * power * UnityEngine.Random.Range(0.9f,1.1f));
                         target.CurrentHP -= damage;
                         Debug.Log($"{user.Profile.displayName} targets {target.Profile.displayName} for {damage} damage");
                         return;
                    case EffectType.MagicDamage:

                         if (element is null)
                         {
                              Debug.LogAssertion("Element cannot be null in a damage effect");
                              return;
                         }
                         finalAttack = motionValue * elementalVulnerability;
                         rawDamage = Clamp0(finalAttack - DEF / 2) * finalAttack / (finalAttack + DEF);
                         power = SPR * (level + 5) / 50;
                         damage = (int)(rawDamage * power * UnityEngine.Random.Range(0.7f, 1.3f));
                         target.CurrentHP -= damage;
                         Debug.Log($"{user.Profile.displayName} targets {target.Profile.displayName} for {damage} damage");
                         return;
                    case EffectType.DefaultHeal:
                         // if target is undead damage them instead
                         float heal = (motionValue + MDEF) * SPR * level;
                         user.CurrentHP += (int)heal;
                         return;
                    case EffectType.ManaRegen:
                         target.CurrentMP += (int)motionValue;
                         return;
                    case EffectType.ConstHeal:
                         target.CurrentHP += (int)motionValue;
                         return;
               }

          }
          public enum EffectType
          {
               MeleeDamage,
               MagicDamage,
               DefaultHeal,
               ManaRegen,
               ConstHeal,
               Resurrect,
               DealStatusDamage,
          }
          [System.Serializable]
          class Effect
          {
               [SerializeField] EffectType effectType;
               [SerializeField] float motionValue;
               
               public EffectType EffectType { get { return effectType; }  }
               public float MotionValue { get {  return motionValue; } }
          }
          float LevelDamageMultiplier(float level)
          {
               ///Don't worry too hard about this until the game is actually functional

               /// I was looking at transitiion functions, and they seem like a really cool way of smoothing between diffent damage functions.
               /// However, beyond different curves to transition between they get extremely complicated. For the sake of simplicity I will just use good old heavyside.
               const float aScaling = 1.08f;
               const float a5 = aScaling * aScaling * aScaling * aScaling * aScaling;
               const float a15 = a5 * a5 * a5;

               const float bScaling = 1.06f;
               const float b5 = bScaling * bScaling * bScaling * bScaling * bScaling;
               const float b15 = b5 * b5 * b5;

               const float cScaling = 1.04f;
               // Mardek 1 - 2 scaling
               if (level <= 15)
                    return MathF.Pow(aScaling, level);
               else if (level <= 30)
                    return a15 + MathF.Pow(bScaling, level - 15);
               else
                    return a15 + b15 + MathF.Pow(cScaling, level - 30);
          }
          int Clamp0(int input)
          {
               return input > 0 ? input : 0;
          }
          float Clamp0(float input)
          {
               return input > 0 ? input : 0;
          }
     }
}