using MARDEK.Audio;
using MARDEK.CharacterSystem;
using MARDEK.Inventory;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MARDEK.UI
{
    using Progress;
    public class BattleLootEntry : BattleLootSelectable
    {
        [SerializeField] AudioObject rejectSound;
        [SerializeField] Image crystalPointer;
        [SerializeField] Image selectedVeil;
        [SerializeField] Image itemImage;
        [SerializeField] TMP_Text itemName;
        [SerializeField] TMP_Text amountText;
        [SerializeField] List<TMP_Text> currentAmountTexts;
          [SerializeField] PartySO party;

        int entryIndex;
        Item[] items;
        int[] amounts;

        public void SetItem(int entryIndex, Item[] items, int[] amounts)
        {
            this.entryIndex = entryIndex;
            this.items = items;
            this.amounts = amounts;

            UpdateItem();
        }
        void DisableDIsplay()
        {
               gameObject.SetActive(false);
               foreach (TMP_Text currentAmountText in currentAmountTexts) currentAmountText.text = "";
                    gameObject.SetActive(false);
          }

        void UpdateItem()
        {
               if (entryIndex >= items.Length||items[entryIndex] is null)
               {
                    DisableDIsplay();
                    return;
               }
               if (amounts[entryIndex] == 0)
               {
                    foreach (TMP_Text currentAmountText in currentAmountTexts) currentAmountText.text = "";
                         gameObject.SetActive(false);
                    return;
               }
               
                itemImage.sprite = items[entryIndex].sprite;
                itemName.text = items[entryIndex].displayName;
                amountText.text = "x " + amounts[entryIndex];
                for (int index = 0; index < currentAmountTexts.Count; index++) 
                {
                    if (index < party.Count)
                    {
                        currentAmountTexts[index].text = party[index].Inventory.CountItem(items[entryIndex]).ToString();
                    }
                    else currentAmountTexts[index].text = "";
                }
        }

        void UpdateCurrentAmountTexts()
        {
            foreach (TMP_Text currentAmountText in currentAmountTexts) currentAmountText.color = itemName.color;
        }
        
        void SetUnselected()
        {
            crystalPointer.gameObject.SetActive(false);
            selectedVeil.gameObject.SetActive(false);
            itemName.color = new Color(239f / 255f, 203f / 255f, 127f / 255f);
            amountText.color = itemName.color;
            UpdateCurrentAmountTexts();
        }

        void SetSelected()
        {
            crystalPointer.gameObject.SetActive(true);
            selectedVeil.gameObject.SetActive(true);
            itemName.color = new Color(153f / 255f, 204f / 255f, 1f);
            amountText.color = new Color(153f / 255f, 1f, 1f);
            UpdateCurrentAmountTexts();
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
            if (CharacterSelectable.currentSelected.Character.Inventory.AddItem(items[entryIndex], amounts[entryIndex]))
            {
                    amounts.RemoveAt(entryIndex);
                    items.RemoveAt(entryIndex);
            }
            else
            {
                AudioManager.PlaySoundEffect(rejectSound);
            }
        }
    }
}
