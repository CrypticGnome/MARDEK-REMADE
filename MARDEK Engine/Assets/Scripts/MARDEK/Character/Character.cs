using UnityEngine;

namespace MARDEK.CharacterSystem
{
    using Core;
    using Inventory;
     using Stats;
     [System.Serializable]
    public class Character : IActor, IStats
    {
        [SerializeField] public bool isRequired;
        [field: SerializeField] public CharacterProfile Profile { get; private set; }
        [field: SerializeField] public Inventory EquippedItems { get; private set; } = new Inventory();
        [field: SerializeField] public Inventory Inventory { get; private set; } = new Inventory();
        public CharacterStats BaseStats { get { return Profile.Stats;} set { Profile.Stats = value; } }

        public int Attack 
        {
               get 
               {
                    int attack = 0;
                    for (int itemIndex = 0; itemIndex < EquippedItems.Slots.Count; itemIndex++)
                    {
                         EquippableItem item = EquippedItems.Slots[itemIndex].item as EquippableItem;
                         if (item is null)
                              continue;
                         attack += item.Stats.Attack;
                    }
                    return attack;
               }
        }
          public int Defense
          {
               get
               {
                    int defense = 0;
                    for (int itemIndex = 0; itemIndex < EquippedItems.Slots.Count; itemIndex++)
                    {
                         EquippableItem item = EquippedItems.Slots[itemIndex].item as EquippableItem;
                         defense += item.Stats.Defense;
                    }
                    return defense;
               }
          }

          public Character Clone(int level)
        {
            var clone = new Character();
            clone.Profile = Profile;
            clone.BaseStats.Level = level;
            return clone;
        }

        public int GetStat(IntegerStat desiredStat)
        {
           return 0;
        }
    }
}