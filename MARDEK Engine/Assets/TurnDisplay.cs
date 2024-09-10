using MARDEK.Battle;
using MARDEK.CharacterSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MARDEK.UI
{
     public class TurnDisplay : MonoBehaviour
     {
          [SerializeField] GameObject[] heroIcons, enemyIcons;
          [SerializeField] RectTransform[] heroTransforms, enemyTransforms;
          [SerializeField] RectTransform displayTransform;
          List<BattleCharacter> playerParty { get => BattleManager.PlayerBattleParty; }
          List<BattleCharacter> enemyParty { get => BattleManager.EnemyBattleParty; }
          float displayWidth;
          private void Awake()
          {
               displayWidth = displayTransform.rect.width;
               Debug.LogWarning("Setting the sprites of the turn display is not yet implemented");
               BattleManager.OnTurnEnd += TrackDisplay;
          }

          private void OnDisable()
          {
               BattleManager.OnTurnEnd -= TrackDisplay;
          }
          [ContextMenu("Set display")]
          void TrackDisplay()
          {
               displayWidth = displayTransform.rect.width;

               for (int i = 0; i < heroIcons.Length; i++)
               {
                    if (i < playerParty.Count)
                         heroIcons[i].SetActive(true);
                    else
                         heroIcons[i].SetActive(false);
               }
               for (int i = 0; i < enemyIcons.Length; i++)
               {
                    if (i < enemyParty.Count)
                         enemyIcons[i].SetActive(true);
                    else
                         enemyIcons[i].SetActive(false);
               }
               SetTurnDisplay();
               StartCoroutine(UpdateDisplay());

               IEnumerator UpdateDisplay()
               {
                    while (BattleManager.instance.state == BattleManager.BattleState.Idle)
                    {
                         SetTurnDisplay();
                         yield return null;
                    }
               }
               void SetTurnDisplay()
               {
                    for (int i = 0; i < playerParty.Count; i++)
                    {
                         float completion = (float)playerParty[i].ACT / TurnManager.ActResolution;
                         heroTransforms[i].anchoredPosition = new Vector2(completion * displayWidth, 0);
                    }

                    for (int i = 0; i < enemyParty.Count; i++)
                    {
                         float completion = (float)enemyParty[i].ACT / TurnManager.ActResolution;
                         enemyTransforms[i].anchoredPosition = new Vector2(completion * displayWidth, 0);
                    }
               }
          }
          
     }
}
