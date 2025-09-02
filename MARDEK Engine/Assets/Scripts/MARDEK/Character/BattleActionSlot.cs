
using UnityEngine;

namespace MARDEK.CharacterSystem
{
    using Core;
     using MARDEK.Skill;
     using MARDEK.Battle;
     using System;
     using MARDEK.Inventory;

     [Serializable]
    public class BattleActionSlot 
    {
          public string DisplayName;
          public Sprite Sprite;
          public int Number;
          public string Description;
          public IBattleAction Action;

          public void ApplyAction(BattleCharacter target) => BattleManager.PerformActionToTarget(Action, target);


          public BattleActionSlot(ActionSkill skill)
          {
               BattleAction action = skill.Action;
               if (action.Element is null)
               {
                    Debug.LogAssertion($"{skill.DisplayName} does not have an attatched element");
                    return;
               }
               DisplayName = skill.DisplayName;
               Sprite = action.Element.thickSprite;
               Number = skill.Cost;
               Description = skill.Description;
               Action = skill;
          }

          public BattleActionSlot(InventorySlot inventorySlot)
          {
               ExpendableItem item = inventorySlot.item as ExpendableItem;
               DisplayName = item.displayName;
               Sprite = item.sprite;
               Number = inventorySlot.Number;
               Description = item.description;
               Action = item;
          }
     }
}