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
          public ApplyAction PerformAction;
          public void ApplyAction()
          {
              BattleManager.PerformAction(PerformAction);
          }
          public BattleActionSlot(ActionSkill skill)
          {
               try
               {
                    DisplayName = skill.DisplayName;
                    Sprite = skill.Element.thickSprite;
                    Number = skill.Cost;
                    Description = skill.Description;
                    PerformAction = skill.Apply;
               }
               catch
               {
                    Debug.LogAssertion($"{skill.name} failed to be displayed as a {this}");
               }
          }

          public BattleActionSlot(Slot inventorySlot)
          {
               Item item = inventorySlot.item;
               DisplayName = item.displayName;
               Sprite = item.sprite;
               Number = inventorySlot.Number;
               Description = item.description;
               PerformAction = Action;
               void Action(Character a, Character b)
               {
                    throw new NotImplementedException();
               }
          }
     }
}