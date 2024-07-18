using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MARDEK.Stats;
using MARDEK.CharacterSystem;
using Newtonsoft.Json.Bson;

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
               characterUI.character.Character.OnStatChanged += UpdateBar;
               UpdateBar();
          }

          [ContextMenu("Update Bar")]
          void UpdateBar()
          {
               if (characterUI.character == null)
                    return;
               Character character = characterUI.character.Character;
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