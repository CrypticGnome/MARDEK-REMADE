using UnityEngine;
using MARDEK.Stats;
using System.Collections.Generic;
using System;
//using MARDEK.CharacterSystem;

namespace MARDEK.Inventory
{
    [CreateAssetMenu(menuName = "MARDEK/Inventory/EquippableItem")]
    public class EquippableItem : Item
    {
        [SerializeField] EquipmentCategory _category;
        [SerializeField] ItemStats statBoosts;
        public ItemStats Stats { get { return statBoosts; } }

        [SerializeField] string _hitSFX;
        public string hitSFX { get { return _hitSFX; } }

        public EquipmentCategory category { get { return _category; } set{_category=value;} }

        
        public EquippableItem()
        {
        }

        public override bool CanStack()
        {
            return false;
        }

        override public Color GetInventorySpaceColor()
        {
            return category.color;
        }
        
    }
}
