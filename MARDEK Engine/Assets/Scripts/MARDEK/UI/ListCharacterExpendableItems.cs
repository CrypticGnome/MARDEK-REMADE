using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.UI
{
    using Battle;
    public class ListCharacterExpendableItems : ListBattleActions
    {
          [SerializeField] Selectable thisSelectable;
          public void SetSlots()
          {
               if (!thisSelectable.Selected) return;

               if (BattleManager.characterActing is not HeroBattleCharacter actingHero)
                    return;

               ClearSlots();
               foreach (var slot in actingHero.Character.Inventory.Slots)
               {
                   if (slot.item is Inventory.ExpendableItem)
                   {
                       SetNextSlot( new CharacterSystem.BattleActionSlot(slot));
                   }
               }
               UpdateLayout();
          }
    }
}