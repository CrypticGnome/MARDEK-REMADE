using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MARDEK.Stats;
using MARDEK.CharacterSystem;
using MARDEK.Battle;

namespace MARDEK.UI
{
    public class HealthBar : MonoBehaviour
    {
          [SerializeField] CharacterUI characterUI = null;
          [SerializeField] RectTransform barTransform;
          [SerializeField] TMPro.TMP_Text statText;
          [SerializeField] TMPro.TMP_Text maxStatText;

          private void Awake()
          {
               characterUI.OnInitialisation += Initialise;
          }
          void Initialise()
          {
               if (characterUI.character is null)
                    return;
               characterUI.character.OnStatChanged += UpdateBar;
               UpdateBar();
          }
          private void Update()
          {
               // Currently when a stat changes the OnStatChanged delegate isn't fired. Therefore, the healthbar doesn't update.
               // Instead of fixing that I'm just doing a quick band aid fix. Future work.
               UpdateBar();
          }
          [ContextMenu("Update Bar")]
          void UpdateBar()
          {
               if (characterUI.character == null)
                    return;
               BattleCharacter character = characterUI.character;
               var statValue = (float)character.CurrentHP;
               var maxStatValue = (float)character.MaxHP;
               if (statText)
                    statText.text = statValue.ToString();
               if (maxStatText) 
                    maxStatText.text = maxStatValue.ToString();
               if (barTransform)
               {
                    float xScale = Mathf.Clamp(statValue / maxStatValue , 0f, 1f);
                    if(float.IsFinite(xScale))
                         barTransform.localScale = new Vector3(xScale, 1, 1);
               }
          }
    }
}