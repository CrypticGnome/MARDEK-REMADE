using System;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace MARDEK.Stats
{

     [Serializable]
     public class CoreStats
     {
          public int Strength, Vitality, Spirit, Agility;
          [Space]
          public Absorbtions Absorbtions;
          public StatusEffects Resistances;
          [Space] 
          public int Attack;
          public int Defense, MagicDefense;
          public int Crit;
          public int Accuracy = 100;
          [Space]
          public MaxHpCalculator MaxHP;
          public MaxMpCalculator MaxMP;
     }
     [Serializable]
     public class Absorbtions
     {
          public int Fire, Earth, Water, Air, Light, Dark, Aether, Fig, Physical, Thauma, Divine;

          public int GetAbsorbtion(ElementID element)
          {
               switch (element) {
                    default: throw new NotImplementedException();
                    case ElementID.Fire: return Fire;
                    case ElementID.Earth: return Earth;
                    case ElementID.Water: return Water;
                    case ElementID.Air: return Air;

                    case ElementID.Light: return Light;
                    case ElementID.Dark: return Dark;
                    case ElementID.Aether: return Aether;
                    case ElementID.Fig: return Fig;

                    case ElementID.Physical: return Physical;
                    case ElementID.Thauma: return Thauma;
                    case ElementID.Divine: return Divine;
               }
          }
     }

     [Serializable]
     public class StatusEffects
     {
          [SerializeField] int poison,  // Take damage at the end of your turn  // Old: 5% dmg turn start, 2% out of combat
               sleep,         // Immobilised until hit or it wears off
               paralysis,     // Skip every other turn
               blindness,     // Cannot choose the target, accuracy is reduced // Old: Accuracy is halved
               silence,       // Disables magic attacks
               numbness,      // Disables physical attacks
               curse,         // HOW IS THIS DIFFERENT TO SILENCE?
               confusion,     // Attack randomly, both allies and enemies
               bleed,         //Take damage at the start of your turn // Old: 10% dmg turn start
               zombification; // Attack allies with basic attacks, take damage from healing.

          public int Poison { get { return  poison; } set { poison = Math.Clamp(value, 0, int.MaxValue); }  }
          public int Sleep { get { return sleep; } set { sleep = Math.Clamp(value, 0, int.MaxValue); } }
          public int Paralysis { get {  return paralysis; } set { paralysis = Math.Clamp(value, 0, int.MaxValue); } }
          public int Blindness { get {  return blindness; } set { blindness =  Math.Clamp(value, 0,int.MaxValue); } }
          public int Silence { get {  return silence; } set { silence =  Math.Clamp(value, 0,int.MaxValue);} }
          public int Numbness { get { return numbness;} set { numbness = Math.Clamp(value,0,int.MaxValue); } }
          public int Curse { get { return curse; } set { curse = Math.Clamp(value, 0, int.MaxValue); } }
          public int Confusion { get { return confusion; } set { confusion = Math.Clamp(value, 0, int.MaxValue); } }
          public int Bleed { get { return bleed; } set { bleed = Math.Clamp(value, 0, int.MaxValue); } }
          public int Zombification { get {  return zombification; } set { zombification =  Math.Clamp(value, 0,int.MaxValue);}}


     }
     [Serializable]
     public class ItemStats
     {    
          public int Attack, Defense, MagicDefense;
          public Absorbtions Absorbtions;
          public StatusEffects Resistances;
          public int Strength, Vitality, Spirit, Agility;
          public int Crit;
          public int Accuracy = 100;
          public int Health, Mana;
     }
     public enum StatusEffect
     {
          None,
          Poison,  // Take damage at the end of your turn  // Old: 5% dmg turn start, 2% out of combat
          Sleep,         // Immobilised until hit or it wears off
          Paralysis,     // Skip every other turn
          Blindness,     // Cannot choose the target, accuracy is reduced // Old: Accuracy is halved
          Silence,       // Disables magic attacks
          Numbness,      // Disables physical attacks
          Curse,         // HOW IS THIS DIFFERENT TO SILENCE?
          Confusion,     // Attack randomly, both allies and enemies
          Bleed,         //Take damage at the start of your turn // Old: 10% dmg turn start
          Zombification // Attack allies with basic attacks, take damage from healing.
     }
}

