using UnityEngine;
using MARDEK.Stats;
using MARDEK.Battle;
using MARDEK.CharacterSystem;

namespace MARDEK.Inventory
{
    [CreateAssetMenu(menuName = "MARDEK/Inventory/ExpendableItem")]
    public class ExpendableItem : Item
     { 
        [SerializeField] string _colorHexCode;
        [SerializeField] string _pfx;

          [SerializeField] Action action;
          public Action Action { get{ return action; } }

        public ExpendableItem(){
            //statsSet = new StatsSet();
        }

        override public Color GetInventorySpaceColor()
        {
            return new Color(81f / 255f, 113f / 255f, 217f / 255f);
        }
     }
}