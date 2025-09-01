using UnityEngine;
using MARDEK.Stats;
using MARDEK.Battle;
using MARDEK.CharacterSystem;

namespace MARDEK.Inventory
{
    [CreateAssetMenu(menuName = "MARDEK/Inventory/ExpendableItem")]
    public class ExpendableItem : Item, IBattleAction
     { 
        [SerializeField] string _colorHexCode;
        [SerializeField] string _pfx;

          [SerializeField] BattleAction action;
          public BattleAction Action { get{ return action; } }

          public Sprite ActionIcon => sprite;

          public string DisplayName => displayName;

          public ExpendableItem(){
            //statsSet = new StatsSet();
        }

        override public Color GetInventorySpaceColor()
        {
            return new Color(81f / 255f, 113f / 255f, 217f / 255f);
        }

          public bool TryPerformAction(BattleCharacter user, BattleCharacter target)
          {
               action.Apply(user, target);
               return true;
          }
     }
}