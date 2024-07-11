using UnityEngine;
using System.Collections.Generic;

namespace MARDEK.CharacterSystem
{
    using Core;
    using Inventory;
     using log4net.Core;
     using Stats;
     [System.Serializable]
    public class Character : IActor
    {
        [SerializeField] public bool isRequired;
        [field: SerializeField] public CharacterProfile Profile { get; private set; }
        [field: SerializeField] public Inventory EquippedItems { get; private set; } = new Inventory();
        [field: SerializeField] public Inventory Inventory { get; private set; } = new Inventory();
        [Header("Stats")]

        [SerializeField] int _level = 1;
        [SerializeField] int _exp = 0;
        int Exp
        {
            get
            {
                return _exp;
            }
            set
            {
                _exp = value;
            }
        }
        public StatsClass Stats { get { return Profile.Stats; } }
        [SerializeField] int _currentHP = -1;

        int CurrentHP
        {
            get
            {
                if (_currentHP == -1 || _currentHP > MaxHP)
                {
                    _currentHP = MaxHP;
                }
                return _currentHP;
            }
            set
            {
               Stats.CurrentHP = value;
                _currentHP = value;
            }
        }
        int MaxHP
        {
            get
            {
                    int VIT = Stats.Vitality;
                    int LVL = Stats.CurrentLevel;
                    return (3 * VIT) + (2 * VIT * LVL);
            }
        }
        
        [SerializeField] int _currentMP = -1;
        int CurrentMP
        {
            get
            {
                if (_currentMP == -1 || _currentMP > MaxMP)
                    _currentMP = MaxMP;
                return _currentMP;
            }
            set
            {
                    Stats.CurrentMP = value;
                _currentMP = value;
            }
        }
        int MaxMP
        {
            get
            {
                    int SPR = Stats.Spirit;
                    int LVL = Stats.CurrentLevel;
                    return SPR * (17 + LVL) / 6;
            }
        }

        public Character Clone(int level)
        {
            var clone = new Character();
            clone.Profile = Profile;
            clone.Stats.CurrentLevel = level;
            return clone;
        }

        public int GetStat(IntegerStat desiredStat)
        {
           return 0;
        }
    }
}