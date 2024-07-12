using UnityEngine;
using MARDEK.Stats;
using System.Collections.Generic;
using MARDEK.Stats;
//using MARDEK.CharacterSystem;

namespace MARDEK.Inventory
{
    [CreateAssetMenu(menuName = "MARDEK/Inventory/EquippableItem")]
    public class EquippableItem : Item
    {
        [SerializeField] EquipmentCategory _category;
       // [SerializeField] StatsSet _statsSet;
        
        // TODO Skills
        // TODO Status effects

        // Temporary field until SFX are implemented to mark which ones belong to which weapons. Remove for final build.
        [SerializeField] string _hitSFX;
        public string hitSFX { get { return _hitSFX; } }

        // Temporary fields only for use during import
        /*[SerializeField] int _ATK;
        public int ATK { get { return _ATK; } }
        [SerializeField] int _CRIT;
        public int CRIT { get { return _CRIT; } }
        [SerializeField] int _DEF;
        public int DEF { get { return _DEF; } }
        [SerializeField] int _EVA;
        public int EVA { get { return _EVA; } }
        [SerializeField] int _HIT;
        public int HIT { get { return _HIT; } }
        [SerializeField] int _MDEF;
        public int MDEF { get { return _MDEF; } }
        [SerializeField] int _SPR;
        public int SPR { get { return _SPR; } }
        [SerializeField] int _STR;
        public int STR { get { return _STR; } }
        [SerializeField] int _VIT;
        public int VIT { get { return _VIT; } }
        [SerializeField] int _MaxMP;
        public int MaxMP { get { return _MaxMP; } }
        [SerializeField] int _MaxHP;
        public int MaxHP { get { return _MaxHP; } }
        [SerializeField] string _categoryText;
        public string categoryText { get { return _categoryText; } set{_categoryText = value;}}*/

        public EquipmentCategory category { get { return _category; } set{_category=value;} }

        //public StatsSet statBoosts { get { return _statsSet; } private set{_statsSet = value;} }
        
        public EquippableItem(){
            //statBoosts = new StatsSet();
        }

        public override bool CanStack()
        {
            return false;
        }

        override public Color GetInventorySpaceColor()
        {
            return category.color.ToColor();
        }
    }
}
