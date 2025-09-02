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
          [SerializeField] Text text;
        public void DisplayAction(IBattleAction skill)
        {
               image.sprite = skill.ActionIcon;
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
