using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MARDEK.CharacterSystem;
using MARDEK.Skill;
using UnityEngine.UI;
using TMPro;

namespace MARDEK.UI
{
    public class ListCharacterSkillset : ListActions
    {
        ActionSkillset skillsetToShow;
        [SerializeField] TextMeshProUGUI skillsetNameLabel = null;
        [SerializeField] Image skillsetIcon = null; 

        private void OnEnable()
        {
            var character = Battle.BattleManager.characterActing;
            skillsetToShow = null;
            skillsetToShow = character.Character.ActionSkillset;

            if (skillsetToShow)
            {
                skillsetNameLabel.text = skillsetToShow.name;
                skillsetIcon.sprite = skillsetToShow.Sprite;
            }
            else
            {
                skillsetNameLabel.text = " - ";
                skillsetIcon.sprite = null;
            }
        }

          public void SetSlots()
          {
               ClearSlots();
               if (!skillsetToShow)
               {
                    Debug.LogWarning("No skillset to show");
                    return;
               }

               foreach (var skill in skillsetToShow.Skills)
               {
                   BattleActionSlot slot = new BattleActionSlot(skill);

                   SetNextSlot(slot);
               }
               UpdateLayout();
          }
    }
}