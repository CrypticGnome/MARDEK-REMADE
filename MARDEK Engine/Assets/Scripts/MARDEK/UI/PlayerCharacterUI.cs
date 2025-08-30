using MARDEK.Animation;
using MARDEK.Battle;
using MARDEK.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCharacterUI : MonoBehaviour, IPointerClickHandler
{
     public HeroBattleCharacter character { get; private set; }
     [SerializeField] GameObject basePanel;
     [SerializeField] Image elementImage;
     [SerializeField] UISpriteAnimator characterAnimator;
     [SerializeField] Text characterName;
     [SerializeField] HealthBar healthBar;
     [SerializeField] ManaBar manaBar;
     [SerializeField] ExperienceBar expBar;

     private void Start()
     {
          UpdateCharacter();
          if (character == null)
          {
               basePanel.SetActive(false);
               return;
          }
          manaBar.SetCharacter(character);
          healthBar.SetCharacter(character);
          expBar.SetCharacter(character);
          characterName.text = character.Name;
          elementImage.sprite = character.Profile.element.thinSprite;
          characterAnimator.ClipList = character.Profile.WalkSprites;
          if (!characterAnimator.enabled) characterAnimator.enabled = true;
     }

     void UpdateCharacter()
     {
          basePanel.SetActive(false);
          var index = transform.GetSiblingIndex();

          if (index < BattleManager.PlayerBattleParty.Count)
          {
               character = (HeroBattleCharacter)BattleManager.PlayerBattleParty[index];
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
