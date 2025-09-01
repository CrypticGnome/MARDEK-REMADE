using MARDEK.Stats;
using System;
using UnityEngine;

namespace MARDEK.Battle
{
     [Serializable]
     public abstract class ActionEffects
     {
          public abstract void ApplyEffect(BattleCharacter user, BattleCharacter target, Element element);

          protected int CalculateDamageStandard(float finalAttack, float defense, float powerStat, int level, float varianceDecimal)
          {
               ///// Helps smooth out the awkwardness of def being too weak at low values and too strong at high values (default: Clamp0(ATK - DEF))
               //
               ///// Power scaling linearly is not ideal. However, with attribute raising potions in the game having non-linear scaling would be dangerous. More thought needed
               //float power = STR * LevelDamageMultiplier(level);

               if (finalAttack + defense == 0) return 0;
               float attackAfterDefense = (finalAttack - defense / 2);

               float rawDamage = attackAfterDefense   * finalAttack / (finalAttack + defense);

               float power = powerStat * (level + 5) / 50;
               return (int)(rawDamage * power * UnityEngine.Random.Range(1- varianceDecimal, 1+ varianceDecimal));
          }

          protected float GetElementalVulnerabilty(BattleCharacter character, Element element)
          {
               int elementalAbsorbtion = character.Absorbtions.GetAbsorbtion(element.ElementID);
               return 1 - (float)elementalAbsorbtion / 100;
          }

          protected float LevelDamageMultiplier(int level)
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
     }
     [Serializable]
     /// <summary>
     ///  Deal damage using the standard to a target
     /// </summary>
     public class DealMeleeDamageStandard : ActionEffects
     {
          public float motionValue;
          public float AccuracyRating = 1;
          public override void ApplyEffect(BattleCharacter user, BattleCharacter target, Element element)
          {
               if (element is null)
               {
                    Debug.LogAssertion("Element cannot be null in a damage effect");
                    return;
               }

               // Should implement evasion
               bool attackMissed = UnityEngine.Random.Range(0, 1) > AccuracyRating;
               if (attackMissed)
               {
                    Debug.Log($"{user.Profile.displayName} misses {target.Profile.displayName}");
                    return;
               }

               float finalAttack = motionValue * user.Attack * GetElementalVulnerabilty(target, element);

               int damage = CalculateDamageStandard(
                    finalAttack: finalAttack,
                    defense: target.Defense,
                    powerStat: user.Strength,
                    level: user.Level,
                    varianceDecimal: 0.1f);

               target.CurrentHP -= damage;
               target.CurrentHP = Mathf.Clamp(target.CurrentHP, 0, target.MaxHP);
               target.battleModel.DamageDisplay.DisplayHPChange(-damage);
               Debug.Log($"{user.Profile.displayName} targets {target.Profile.displayName} for {damage} damage");
          }
     }

     public class DealMagicDamageStandard : ActionEffects
     {
          public float motionValue;
          public override void ApplyEffect(BattleCharacter user, BattleCharacter target, Element element)
          {
               float elementalVulnerability = GetElementalVulnerabilty(target, element);

               if (element is null)
               {
                    Debug.LogAssertion("Element cannot be null in a damage effect");
                    return;
               }
               float finalAttack = user.Attack * motionValue * elementalVulnerability;

               int damage = CalculateDamageStandard(
                    finalAttack: finalAttack,
                    defense: target.MagicDefense,
                    powerStat: user.Spirit,
                    level: user.Level,
                    varianceDecimal: 0.1f);

               target.CurrentHP -= damage;
               target.CurrentHP = Mathf.Clamp(target.CurrentHP, 0, target.MaxHP);
               target.battleModel.DamageDisplay.DisplayHPChange(-damage);

               Debug.Log($"{user.Profile.displayName} targets {target.Profile.displayName} for {damage} damage");
          }
     }

     public class DefaultHeal : ActionEffects
     {
          public float numeratorMotionValue, denominatorMotionValue;
          public override void ApplyEffect(BattleCharacter user, BattleCharacter target, Element element)
          {
               float elementalVulnerability = GetElementalVulnerabilty(target, element);

               int heal = (int)((numeratorMotionValue + target.MagicDefense) * user.Spirit * user.Level / denominatorMotionValue);

               if (target.Profile.Type == CharacterSystem.CharacterType.Undead)
                    heal *= -1;

               target.CurrentHP = Mathf.Clamp(target.CurrentHP + heal, 0, target.MaxHP);
               target.battleModel.DamageDisplay.DisplayHPChange(heal);
               Debug.Log($"{user.Profile.displayName} heals {target.Profile.displayName} for {heal} hp");
          }
     }
     public class ManaHeal: ActionEffects
     {
          public float MotionValue;
          public override void ApplyEffect(BattleCharacter user, BattleCharacter target, Element element)
          {
               int manaHeal = (int)MotionValue;
               target.CurrentMP += manaHeal;
               target.battleModel.DamageDisplay.DisplayMPChange(manaHeal);

          }
     }
     public class ConstHeal : ActionEffects
     {
          public float MotionValue;
          public override void ApplyEffect(BattleCharacter user, BattleCharacter target, Element element)
          {
               int heal = (int)MotionValue;
               target.CurrentHP += heal;
               target.battleModel.DamageDisplay.DisplayHPChange(heal);
          }
     }
     public class DealStatusDamage : ActionEffects
     {
          public int MotionValue;
          public StatusEffect StatusEffect;
          public override void ApplyEffect(BattleCharacter user, BattleCharacter target, Element element)
          {
               int resistance = target.Resistances.Get(StatusEffect);
               int currentBuildUp = target.StatusBuildup.Get(StatusEffect);
               target.StatusBuildup.Set(StatusEffect, currentBuildUp + MotionValue);

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
}