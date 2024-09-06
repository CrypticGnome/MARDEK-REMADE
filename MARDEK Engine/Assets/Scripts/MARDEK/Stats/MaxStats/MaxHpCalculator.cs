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
          public int GetMaxHP(Character character)
          {
               int vitality = character.BaseStats.Vitality;
               switch (calculatorType)
               {
                    default:
                         return 3 * vitality + (2 * vitality * character.Level);
                    case CalculatorType.Constant:
                         return constantValue;
                    case CalculatorType.Monster:
                         return 28 * character.Level + 1172;
               }
          }
          public int GetMaxHP(BattleCharacter character)
          {
               int vitality = character.Vitality;
               const int LEVEL = 50;
               int cLevel = character.Level;
               switch (calculatorType)
               {
                    default:
                         int levelContribution = 2 * vitality * cLevel;
                         int levelContribution2 = 2 * vitality * LEVEL;
                         int returnVal = 3 * vitality + levelContribution;
                         return returnVal;
                    case CalculatorType.Constant:
                         return constantValue;
                    case CalculatorType.Monster:
                         return 28 * character.Level + 1172;
               }
          }
          public enum CalculatorType
          {
               Default,
               Constant,
               Monster,
          }
     }
}