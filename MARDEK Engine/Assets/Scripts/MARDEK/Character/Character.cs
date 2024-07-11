using UnityEngine;
using System.Collections.Generic;
using MARDEK.Inventory;
using MARDEK.Stats;

namespace MARDEK.CharacterSystem
{
    [System.Serializable]
    public class Character : IStats
    {
        [SerializeField] public bool isRequired;
        [field: SerializeField] public CharacterProfile Profile { get; private set; }
        [field: SerializeField] public Inventory.Inventory EquippedItems { get; private set; } = new Inventory.Inventory();
        [field: SerializeField] public Inventory.Inventory Inventory { get; private set; } = new Inventory.Inventory();
        [Header("Stats")]
        [SerializeField] StatsSet volatileStats = new StatsSet(true);
        int MaxHP
        {
            get
            {
                return (int)Profile.MaxHPExpression.Evaluate(this, null);
            }
        }
        int MaxMP
        {
            get
            {
                return (int)Profile.MaxMPExpression.Evaluate(this, null);
            }
        }
        [SerializeField] int _currentHP = -1;
        [SerializeField] int _currentMP = -1;
        int CurrentHP
        {
            get
            {
                var maxHP = GetStat(StatsGlobals.Instance.MaxHP);
                if (_currentHP == -1 || _currentHP > maxHP)
                {
                    _currentHP = maxHP;
                }
                return _currentHP;
            }
            set
            {
                _currentHP = value;
            }
        }
        int CurrentMP
        {
            get
            {
                var maxMP = GetStat(StatsGlobals.Instance.MaxMP);
                if (_currentMP == -1 || _currentMP > maxMP)
                    _currentMP = maxMP;
                return _currentMP;
            }
            set
            {
                _currentMP = value;
            }
        }

        public Character Clone()
        {
            var clone = new Character();
            clone.Profile = Profile;
            return clone;
        }

        public int GetStat(IntegerStat desiredStat)
        {            
            if (desiredStat == StatsGlobals.Instance.CurrentHP)
                return CurrentHP;
            if (desiredStat == StatsGlobals.Instance.CurrentMP)
                return CurrentMP;

            var resultHolder = new StatHolder(desiredStat);
            if (desiredStat == StatsGlobals.Instance.MaxHP)
                resultHolder.Value = MaxHP;
            if (desiredStat == StatsGlobals.Instance.MaxMP)
                resultHolder.Value = MaxMP;

            resultHolder.Value += Profile.StartingStats.GetStat(desiredStat);
            resultHolder.Value += volatileStats.GetStat(desiredStat);
            foreach(var slot in EquippedItems.Slots)
            {
                var equippableItem = slot.item as EquippableItem;
                if(equippableItem != null)
                    resultHolder.Value += equippableItem.statBoosts.GetStat(desiredStat);
            }
            return resultHolder.Value;
        }
        public void ModifyStat(IntegerStat stat, int delta)
        {
            if (stat == StatsGlobals.Instance.CurrentHP)
            {
                CurrentHP += delta;
            }
            else if (stat == StatsGlobals.Instance.CurrentMP)
            {
                CurrentMP += delta;
            }
            else
                volatileStats.ModifyStat(stat, delta);
        }
    }
}