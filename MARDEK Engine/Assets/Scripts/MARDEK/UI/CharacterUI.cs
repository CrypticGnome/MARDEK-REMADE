using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using MARDEK.CharacterSystem;
using UnityEngine.EventSystems;

namespace MARDEK.UI
{
    using Battle;
     public class CharacterUI : MonoBehaviour, IPointerClickHandler
     {
          [SerializeField] bool playableOrEnemy;
          public BattleCharacter character { get; private set; }
          [SerializeField] GameObject basePanel;
          public delegate void Initialised();
          public Initialised OnInitialisation;

          private void Start()
          {
               UpdateCharacter();
               OnInitialisation?.Invoke();
          }

          void UpdateCharacter()
          {
               basePanel.SetActive(false);
               var index = transform.GetSiblingIndex();
               List<BattleCharacter> list = BattleManager.EnemyBattleParty;
               if (playableOrEnemy)
                    list = BattleManager.PlayerBattleParty;
               if (index < list.Count)
               {
                    character = list[index];
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