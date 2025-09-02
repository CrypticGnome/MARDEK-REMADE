using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using MARDEK.CharacterSystem;
using UnityEngine.EventSystems;

namespace MARDEK.UI
{
    using Battle;
     public class EnemyCharacterUI : MonoBehaviour, IPointerClickHandler
     {
          public BattleCharacter character { get; private set; }
          [SerializeField] GameObject basePanel;
          [SerializeField] EnemyHealthBar healthBar;
          [SerializeField] Text characterName;
          [SerializeField] Image elementImage;
          [SerializeField] Text levelText;

          private void Start()
          {
               UpdateCharacter();
               if (character == null)
               {
                    basePanel.SetActive(false);
                    return;
               }
               healthBar.SetCharacter(character);
               characterName.text = character.Name;
               elementImage.sprite = character.Profile.element.thickSprite;
               levelText.text = "Lv " + character.Level.ToString();
          }

          void UpdateCharacter()
          {
               basePanel.SetActive(false);
               var index = transform.GetSiblingIndex();

               if (index < BattleManager.EnemyBattleParty.Count)
               {
                    character = BattleManager.EnemyBattleParty[index];
                    basePanel.SetActive(true);
               }
               else
               {
                    character = null;
                    basePanel.SetActive(false);
               }
          }

          public void OnPointerClick(PointerEventData eventData)
          {
               BattleUIManager.Instance.InspectCharacter(character);
          }
    }
}