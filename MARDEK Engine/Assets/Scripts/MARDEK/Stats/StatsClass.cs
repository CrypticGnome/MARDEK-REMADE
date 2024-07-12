using System;

namespace MARDEK.Stats
{
     [Serializable]
     public class StatsClass
     {
          public int Vitality, Strength, Spirit, Agility;
          public int Level, Experience;
          public Absorbtions Absorbtions;
          public float ACT;

          int _currentHP = -1;
          public int CurrentHP 
          {
               get
               {
                    if (_currentHP == -1 || _currentHP > MaxHP.GetMaxHP(this))
                    {
                         _currentHP = MaxHP.GetMaxHP(this);
                    }
                    return _currentHP;
               }
               set
               {
                    _currentHP = value;
               }
          }
          int _currentMP = -1;
          public int CurrentMP
          {
               get
               {
                    if (_currentMP == -1 || _currentMP > MaxMP.GetMaxMP(this))
                         _currentMP = MaxMP.GetMaxMP(this);
                    return _currentMP;
               }
               set
               {
                    _currentMP = value;
               }
          }
          public MaxHpCalculator MaxHP;
          public MaxMpCalculator MaxMP;
     }
     [Serializable]
     public class Absorbtions
     {
          public int Fire, Earth, Water, Air, Light, Dark, Aether, Fig, Physical, Thauma;
     }
}

