using MARDEK.CharacterSystem;
using UnityEngine;

namespace MARDEK.Stats
{
     [CreateAssetMenu(menuName = "MARDEK/Stats/Max MP")]
     public class MaxMpCalculator : ScriptableObject
     {
          [SerializeField] CalculatorType calculatorType;
          [SerializeField] int constantValue;
          public int GetMaxMP(Character character)
          {
               switch (calculatorType)
               {
                    default:
                         return character.BaseStats.Spirit * (17 + character.Level)/6;
                    case CalculatorType.Constant:
                         return constantValue;
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