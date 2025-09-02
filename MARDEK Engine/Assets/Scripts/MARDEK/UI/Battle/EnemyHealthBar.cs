using MARDEK.Battle;
using UnityEngine;

namespace MARDEK.UI
{
    public class EnemyHealthBar : MonoBehaviour
    {
          [SerializeField] RectTransform barTransform;
          [SerializeField] TMPro.TMP_Text statText;
          BattleCharacter character;
          public void SetCharacter(BattleCharacter character)
          {
               if (character is null)
                    return;
               this.character = character;
               character.OnStatChanged += UpdateBar;
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
               if (character == null)
                    return;
               var statValue = (float)character.CurrentHP;
               var maxStatValue = (float)character.MaxHP;
               if (statText)
                    statText.text = statValue.ToString();
               if (barTransform)
               {
                    float xScale = Mathf.Clamp(statValue / maxStatValue, 0f, 1f);
                    if (float.IsFinite(xScale))
                         barTransform.localScale = new Vector3(xScale, 1, 1);
               }
          }
     }
}
