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
          public static BattleActionSlot selectedInstance;
          public static UpdateSelectedSlot UpdateSelected { get; set; }
          [SerializeField] Image sprite;
          [SerializeField] Text nameLabel;
          [SerializeField] Text number;
          BattleActionSlot slot;
          [SerializeField] BattleCharacterPicker targetPicker;

          public void SetSlot(BattleActionSlot _slot)
          {
               slot = _slot;
               if (slot == null)
               {
                    gameObject.SetActive(false);
                    return;
               }

               nameLabel.text = slot.DisplayName;
               sprite.sprite = slot.Sprite;
               number.text = slot.Number.ToString();
               gameObject.SetActive(true);
          }

          public void SelectAction() => targetPicker.EnableWithAction(slot.ActionSkill.Action.Apply);

          public override void Select(bool playSFX = true)
          {
               base.Select(playSFX);
               selectedInstance = slot;
               UpdateSelected?.Invoke(slot);
          }
    }
}