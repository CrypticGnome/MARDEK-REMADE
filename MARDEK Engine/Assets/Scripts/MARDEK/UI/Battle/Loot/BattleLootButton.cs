using MARDEK.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
namespace MARDEK.UI
{
    public class BattleLootButton : BattleLootSelectable
    {
        [SerializeField] Image buttonImage;
        [SerializeField] TMP_Text text;
        [SerializeField] Image crystalPointer;
        [SerializeField] bool getAll;

        void SetUnselected()
        {
            buttonImage.color = new Color(159f / 255f, 95f / 255f, 55f / 255f);
            text.color = new Color(1f, 204f / 255f, 153f / 255f);
            crystalPointer.gameObject.SetActive(false);
        }

        void SetSelected()
        {
            buttonImage.color = new Color(63f / 255f, 100f / 255f, 131f / 255f);
            text.color = new Color(153f / 255f, 204f / 255f, 1f);
            crystalPointer.gameObject.SetActive(true);
        }

        override public void Select(bool playSFX = true)
        {
            base.Select(playSFX);
            SetSelected();
        }

        override public void Deselect()
        {
            base.Deselect();
            SetUnselected();
        }

        public override void Interact(List<Item> items, List<int> amounts)
        {
               if (getAll)
               {
                    TakeAll(items, amounts);
               }
               else
               {
                    // TODO Close this menu I guess
               }
        }
        void TakeAll(List<Item> items, List<int> amounts)
          {
               for (int index = 0; index < items.Count; index++)
               {
                    if (amounts[index] == 0)
                         continue;
                    Inventory.Inventory characterInventory = CharacterSelectable.currentSelected.Character.Inventory;
                    if (characterInventory.AddItem(items[index], amounts[index]))
                    {
                         amounts[index] = 0;
                    }
               }
          }
    }
}
