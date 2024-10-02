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
     using static Codice.CM.Common.CmCallContext;

     [Serializable]
    public class BattleActionSlot 
    {
          public string DisplayName;
          public Sprite Sprite;
          public int Number;
          public string Description;
          public ActionSkill ActionSkill;
          public void ApplyAction()
          {
               if (ActionSkill.Cost > BattleManager.characterActing.CurrentMP)
               {
                    Debug.LogWarning("Cannot afford skill");
                    return;
               }
               int currentMP = BattleManager.characterActing.CurrentMP;
               int newMP = currentMP - ActionSkill.Cost;
               BattleManager.characterActing.CurrentMP = newMP;

               BattleManager.PerformAction(ActionSkill);
          }
          public BattleActionSlot(ActionSkill skill)
          {
               ActionSkill = skill;
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
          }

          public BattleActionSlot(InventorySlot inventorySlot)
          {
               ExpendableItem item = inventorySlot.item as ExpendableItem;
               DisplayName = item.displayName;
               Sprite = item.sprite;
               Number = inventorySlot.Number;
               Description = item.description;
          }
     }
}