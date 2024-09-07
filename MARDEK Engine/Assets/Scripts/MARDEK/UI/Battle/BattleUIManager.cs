using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MARDEK.Battle;
using MARDEK.CharacterSystem;

namespace MARDEK.UI
{
    public class BattleUIManager : MonoBehaviour
    {
        public static BattleUIManager Instance { get; private set; }

        [SerializeField] BattleManager battleManager;
        [SerializeField] GameObject CharacterInspectionCard;
          [SerializeField] RectTransform actionPicker;
        public BattleCharacter characterBeingInspected { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
          private void OnEnable()
          {
               BattleManager.OnTurnEnd += OnTurnEnd;
               BattleManager.OnTurnStart += OnTurnStart;
          }
          private void OnDisable()
          {
               BattleManager.OnTurnEnd -= OnTurnEnd;
               BattleManager.OnTurnStart -= OnTurnStart;
          }
          void OnTurnEnd()
          {
               actionPicker.gameObject.SetActive(false);
          }
          void OnTurnStart()
          {
               actionPicker.gameObject.SetActive(true);
          }
          public void InspectCharacter(BattleCharacter character)
          {
               if (characterBeingInspected == character)
               {
                    characterBeingInspected = null;
                    CharacterInspectionCard.SetActive(false);
                    return;
               }

               characterBeingInspected = character;
               CharacterInspectionCard.SetActive(false);
               CharacterInspectionCard.SetActive(true);
          }
    }
}