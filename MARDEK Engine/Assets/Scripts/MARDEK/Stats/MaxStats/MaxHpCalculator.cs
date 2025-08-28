using MARDEK.Battle;
using MARDEK.CharacterSystem;
using UnityEditor.Search;
using UnityEngine;

namespace MARDEK.Stats
{
     [CreateAssetMenu(menuName = "MARDEK/Stats/Max HP")]
     public class MaxHpCalculator : ScriptableObject
     {
          [SerializeField] CalculatorType calculatorType;
          [SerializeField] int constantValue;
          [SerializeField] int levelMultiplier;
          public int GetMaxHP(Character character)
          {
               int vitality = character.BaseStats.Vitality;
               switch (calculatorType)
               {
                    default:
                         return 3 * vitality + (2 * vitality * character.Level);
                    case CalculatorType.Constant:
                         return constantValue;
                    case CalculatorType.StandardEnemy:
                         return levelMultiplier * character.Level + constantValue;
               }
          }
          public int GetMaxHP(BattleCharacter character)
          {
               int vitality = character.Vitality;
               int cLevel = character.Level;
               switch (calculatorType)
               {
                    default:
                         int levelContribution = 2 * vitality * cLevel;
                         int returnVal = 3 * vitality + levelContribution;
                         return returnVal;
                    case CalculatorType.Constant:
                         return constantValue;
                    case CalculatorType.StandardEnemy:
                         return levelMultiplier * character.Level + constantValue;
               }
          }
          public enum CalculatorType
          {
               Default,
               Constant,
               StandardEnemy,
          }
     }
}