using MARDEK.Skill;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MARDEK.Battle
{
    public class ActionDisplay : MonoBehaviour
    {
          [SerializeField] Image image;
          [SerializeField] TextMeshProUGUI text;
        public void DisplayAction(ActionSkill skill)
        {
               image.sprite = skill.Action.Element.thickSprite;
               text.text = skill.DisplayName;
               gameObject.SetActive(true);
               StartCoroutine(DisplayAction());
        }
        IEnumerator DisplayAction()
        {
               while (BattleManager.instance.state == BattleManager.BattleState.ActionPerforming)
               {
                    yield return null;
               }
               gameObject.SetActive(false);
        }
    }
}
