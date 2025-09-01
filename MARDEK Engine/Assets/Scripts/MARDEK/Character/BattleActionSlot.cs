
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
          public ApplyBattleAction PerformAction;
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

               BattleManager.PerformAction(PerformAction);
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