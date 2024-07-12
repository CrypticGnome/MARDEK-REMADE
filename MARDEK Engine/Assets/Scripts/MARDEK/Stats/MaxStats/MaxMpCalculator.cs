using UnityEngine;

namespace MARDEK.Stats
{
     [CreateAssetMenu(menuName = "MARDEK/Stats/Max MP")]
     public class MaxMpCalculator : ScriptableObject
     {
          [SerializeField] CalculatorType calculatorType;
          public int GetMaxMP(CharacterStats stats)
          {
               switch (calculatorType)
               {
                    default:
                         return stats.CoreStats.Spirit * (17 + stats.Level)/6;
               }
          }

          public enum CalculatorType
          {
               Default,
               Monster,
          }
     }
}