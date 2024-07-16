namespace MARDEK.UI
{
    using Progress;
    using CharacterSystem;
     using System;
     using UnityEngine;
     using MARDEK.Inventory;

     public class CharacterEquipmentUI : MonoBehaviour
    {
        [SerializeField]  SlotUI MainHand, OffHand, Head, Body, Accessory1, Accessory2;
        public Character Character
        {
            get
            {
                var index = transform.GetSiblingIndex();
                if (Party.Instance.Characters.Count <= index)
                    return null;
                return Party.Instance.Characters[index];
            }
        }
        private void OnEnable()
        {
            var index = transform.GetSiblingIndex();
               if (Character != null)
                    DisplayEquippedItems(Character);
        }
          void DisplayEquippedItems(Character character)
          {
               MainHand.SetSlot(character.ItemsEquipped.MainHand);
               OffHand.SetSlot(character.ItemsEquipped.OffHand);
               Head.SetSlot(character.ItemsEquipped.Head);
               Body.SetSlot(character.ItemsEquipped.Body);
               Accessory1.SetSlot(character.ItemsEquipped.Accessory1);
               Accessory2.SetSlot(character.ItemsEquipped.Accessory2);
          }
     }
}