using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.CharacterSystem
{
    using Core;
     using MARDEK.Skill;
     using MARDEK.Battle;
     using Stats;
     using System;
     using MARDEK.Inventory;
     using static PlasticPipe.Server.MonitorStats;

     [Serializable]
    public class BattleActionSlot 
    {
          public string DisplayName;
          public Sprite Sprite;
          public int Number;
          public string Description;
          public ApplyBattleAction PerformAction;
          public void ApplyAction()
          {
              BattleManager.PerformAction(PerformAction);
          }
          public BattleActionSlot(ActionSkill skill)
          {
               Battle.BattleAction action = skill.Action;
               if (action.Element is null)
               {
                    Debug.LogAssertion($"{skill.DisplayName} does not have an attatched element");
                    return;
               }
               DisplayName = skill.DisplayName;
               Sprite = action.Element.thickSprite;
               Number = skill.Cost;
               Description = skill.Description;
               PerformAction = action.Apply;
          }

          public BattleActionSlot(InventorySlot inventorySlot)
          {
               ExpendableItem item = inventorySlot.item as ExpendableItem;
               DisplayName = item.displayName;
               Sprite = item.sprite;
               Number = inventorySlot.Number;
               Description = item.description;
               PerformAction = item.Action.Apply;
          }
     }
}