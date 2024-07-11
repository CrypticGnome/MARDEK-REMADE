using System;

namespace MARDEK.Stats
{
     [Serializable]
     public class StatsClass
     {
          public int Vitality, Strength, Spirit, Agility;
          public int CurrentLevel, Experience;
          public Absorbtions Absorbtions;
          public float ACT;
          public int CurrentHP, CurrentMP;
          public int MaxHP, MaxMP;
     }
     [Serializable]
     public class Absorbtions
     {
          public int Fire, Earth, Water, Air, Light, Dark, Aether, Fig, Physical, Thauma;
     }
}