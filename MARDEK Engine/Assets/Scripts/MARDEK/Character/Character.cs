using UnityEngine;

namespace MARDEK.CharacterSystem
{
     using Codice.Client.GameUI.Status;
     using Core;
    using Inventory;
     using Stats;
     using System;
     using System.Collections;
     using System.Threading;

     [CreateAssetMenu(menuName = "MARDEK/Character/Character")]
     public class Character : ScriptableObject, IActionStats
    {
          [SerializeField] public bool isRequired;
          [field: SerializeField] public CharacterProfile Profile { get; private set; }
          [field: SerializeField] public EquippedItems ItemsEquipped { get; private set; }
          [field: SerializeField] public Inventory Inventory { get; private set; } 
          public CoreStats BaseStats { get { return Profile.Stats;} }
          [field: SerializeField] public ActionSkillset ActionSkillset { get; private set; }
          [field: SerializeField] public ReactionSkillset ReactionSkillset { get; private set; }
          [field: SerializeField] public PassiveSkillset PassiveSkillset { get; private set; }
          public delegate void StatChanged();
          public event StatChanged OnStatChanged;
          public Character()
          {
               Inventory = new Inventory();
          }

          [SerializeField] int attack;
          public int Attack 
          {
               get 
               {
                    attack = BaseStats.Attack;
                    for (int itemIndex = 0; itemIndex < EquippedItems.Count; itemIndex++)
                    {
                         EquippableItem item = ItemsEquipped.Slots[itemIndex].item as EquippableItem;
                         if (item is null)
                              continue;
                         attack += item.Stats.Attack;
                    }
                    return attack;
               }
          }

          [SerializeField] int defense;
          public int Defense
          {
               get
               {
                    defense = BaseStats.Defense;
                    for (int itemIndex = 0; itemIndex < EquippedItems.Count; itemIndex++)
                    {
                         EquippableItem item = ItemsEquipped.Slots[itemIndex].item as EquippableItem;
                         if (item is null)
                              continue;
                         defense += item.Stats.Defense;
                    }
                    return defense;
               }
               set {
                    throw new NotImplementedException();
               }
          }
          [SerializeField] int magicDefense;
          public int MagicDefense
          {
               get
               {
                    magicDefense = BaseStats.MagicDefense;
                    for (int itemIndex = 0; itemIndex < EquippedItems.Count; itemIndex++)
                    {
                         EquippableItem item = ItemsEquipped.Slots[itemIndex].item as EquippableItem;
                         if (item is null)
                              continue;
                         magicDefense += item.Stats.MagicDefense;
                    }
                    return magicDefense;
               }
               set
               {
                    throw new NotImplementedException();
               }
          }
          [SerializeField]int _currentHP;
          public int CurrentHP
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
                    _currentHP = value;
                    OnStatChanged?.Invoke();
               }
          }

          [SerializeField] int _currentMP;
          public int CurrentMP
          {
               get
               {
                    if (_currentMP == -1 || _currentMP > MaxMP)
                         _currentMP = MaxMP;
                    return _currentMP;
               }
               set
               {
                    _currentMP = value;
                    OnStatChanged?.Invoke();
               }
          }
          public int MaxHP{get{ return BaseStats.MaxHP.GetMaxHP(this);}}
          public int MaxMP { get { return BaseStats.MaxMP.GetMaxMP(this); } }


          public Absorbtions Absorbtions { get => BaseStats.Absorbtions; set => throw new NotImplementedException(); }
          public Resistances Resistances { get => BaseStats.Resistances; set => throw new NotImplementedException(); }
          public int Agility { get => BaseStats.Agility; set => throw new NotImplementedException(); }
          public float ACT { get; set; }
          public int Accuracy  { get => BaseStats.Accuracy; set => throw new NotImplementedException(); }
          public int CritRate  { get => BaseStats.Crit; set => throw new NotImplementedException(); }
          public int Strength  { get => BaseStats.Strength; set => throw new NotImplementedException(); }
          public int Vitality { get => BaseStats.Vitality; set => throw new NotImplementedException(); }
          public int Spirit { get => BaseStats.Spirit; set => throw new NotImplementedException(); }

          public int Level;
          public int Experience;
          public Character Clone(int level)
          {
               var clone = new Character();
               clone.Profile = Profile;
               clone.Level = level;
               clone.ActionSkillset = Profile.LearnableSkillset;
               return clone;
          }

        public int GetStat(IntegerStat desiredStat)
        {
           return 0;
        }
        [Serializable]
          public class EquippedItems
          {
               public InventorySlot MainHand, OffHand, Head, Body, Accessory1, Accessory2;
               public InventorySlot[] Slots { get { return new InventorySlot[] { MainHand, OffHand, Head, Body, Accessory1, Accessory2 }; } }
               public static int Count = 6;
          }
     }
}