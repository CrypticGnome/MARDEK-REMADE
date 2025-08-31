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
          [SerializeField, HideInInspector] RectTransform[] heroTransforms = new RectTransform[4], enemyTransforms = new RectTransform[5];
          [SerializeField, HideInInspector] Image[] heroImages = new Image[4], enemyImages = new Image[5];
          [SerializeField] RectTransform displayTransform;

          List<BattleCharacter> playerParty { get => BattleManager.PlayerBattleParty; }
          List<BattleCharacter> enemyParty { get => BattleManager.EnemyBattleParty; }
          float displayWidth;

          private void OnValidate()
          {
               for (int i = 0; i< heroIcons.Length; i++)
               {
                    if (!heroTransforms[i])
                         if (heroIcons[i].TryGetComponent(out heroTransforms[i])) 
                              Debug.LogError($"Failure to validate {heroTransforms[i].name}");
                    if (!heroImages[i]) 
                         if (heroIcons[i].TryGetComponent(out heroImages[i])) 
                         Debug.LogError($"Failure to validate {heroTransforms[i].name}");
               }
               for (int i = 0; i < enemyIcons.Length; i++)
               {
                    if (!enemyTransforms[i])
                         if(enemyIcons[i].TryGetComponent(out enemyTransforms[i])) 
                              Debug.LogError($"Failure to validate {enemyTransforms[i].name}");
                    if (!enemyImages[i])
                         if (enemyIcons[i].TryGetComponent(out enemyImages[i])) 
                              Debug.LogError($"Failure to validate {enemyTransforms[i].name}");
               }
          }
          private void Awake()
          {
               displayWidth = displayTransform.rect.width;
               Debug.LogWarning("Setting the sprites of the turn display is not yet implemented");
               BattleManager.OnTurnEnd += TrackDisplay;
          }
          private void OnEnable()
          {
               SetTurnDisplay();
               StartCoroutine(UpdateDisplay());

               for (int i = 0; i < playerParty.Count; i++)
                    heroImages[i].sprite = playerParty[i].GetBattleIcon();

               for (int i = 0; i < enemyParty.Count; i++)
                    enemyImages[i].sprite = enemyParty[i].GetBattleIcon();


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
          private void OnDestroy()
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
               if (!gameObject.activeSelf)
                    gameObject.SetActive(true);
               else OnEnable();
               
          }
          
     }
}
