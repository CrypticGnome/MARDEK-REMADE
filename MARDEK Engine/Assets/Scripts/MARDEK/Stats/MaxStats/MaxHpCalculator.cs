using UnityEditor.Search;
using UnityEngine;

namespace MARDEK.Stats
{
     [CreateAssetMenu(menuName = "MARDEK/Stats/Max HP")]
     public class MaxHpCalculator : ScriptableObject
     {
          [SerializeField] CalculatorType calculatorType;
          public int GetMaxHP(CharacterStats stats)
          {
               int vitality = stats.CoreStats.Vitality;
               switch (calculatorType)
               {
                    default:
                         return 3 * vitality + (2 * vitality * stats.Level);
                    case CalculatorType.Monster:
                         return 28 * stats.Level + 1172;
               }
          }

          public enum CalculatorType
          {
          Default,
          Monster,
          }
     }
}