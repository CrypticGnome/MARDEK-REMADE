using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MARDEK.CharacterSystem;
using UnityEngine.UI;

namespace MARDEK.UI
{
    using Core;
    public class BattleActionSlotUI : Selectable
    {
        public delegate void UpdateSelectedSlot(BattleActionSlot slot);
        public static UpdateSelectedSlot UpdateSelected { get; set; }
        [SerializeField] Image sprite;
        [SerializeField] Text nameLabel;
        [SerializeField] Text number;
          BattleActionSlot slot;

        public void SetSlot(BattleActionSlot _slot)
        {
            slot = _slot;
            if (slot == null)
                gameObject.SetActive(false);
            else
            {
                nameLabel.text = slot.DisplayName;
                sprite.sprite = slot.Sprite;
                number.text = slot.Number.ToString();
                gameObject.SetActive(true);
            }
        }

          public void SelectAction()
          {
               slot.ApplyAction();
          }

          public override void Select(bool playSFX = true)
        {
            base.Select(playSFX);
            UpdateSelected?.Invoke(slot);
        }
    }
}