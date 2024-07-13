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
          public Resistances Resistances;
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
          public int Fire, Earth, Water, Air, Light, Dark, Aether, Fig, Physical, Thauma;
     }

     [Serializable]
     public class Resistances
     {
          public int Sleep;
     }
     [Serializable]
     public class ItemStats
     {
          public int Attack, Defense, MagicDefense;
          public Absorbtions Absorbtions;
          public Resistances Resistances;
          public CoreStats CoreStats;
          public int Crit;
          public int Accuracy = 100;
     }
}

