using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageDisplay : MonoBehaviour
{
     [SerializeField] Text TextDisplay;
     [SerializeField] RectTransform textTransform;
     Color damageColor = new Color(1, 0.3f, 0.3f, 1),
     manaDamageColor = new Color(1, 0.5f, 1f, 1),
     healColor = new Color(0.3f, 1f, 0.3f, 1),
     manaHealColor = new Color(1f, 1f, 0.3f, 1),
     neutralColor = new Color(0.5f, 0.5f, 0.5f, 1);
     public void DisplayHPChange(int value)
     {
          gameObject.SetActive(true);
          textTransform.localScale = textTransform.transform.position.x < 0 ? new Vector3(-1,1,1) : Vector3.one;
          TextDisplay.color = value == 0 ? neutralColor : 
               value < 0 ? damageColor: healColor;
          TextDisplay.text = Mathf.Abs(value).ToString();
          StartCoroutine(FadeAway());
     }
     public void DisplayMPChange(int value)
     {
          gameObject.SetActive(true);
          TextDisplay.color = value == 0 ? neutralColor :
               value < 0 ? manaDamageColor: manaHealColor;
          TextDisplay.text = Mathf.Abs(value).ToString();
          StartCoroutine(FadeAway());
     }
     IEnumerator FadeAway()
     {
          float timer = 0;
          const float timeToFadeAway = 1.2f;
          Color textColor = TextDisplay.color;
          Vector2 anchorPosition = Vector2.zero;

          while (timer < timeToFadeAway)
          {
               yield return null;
               timer += Time.deltaTime;
               float completion = timer / timeToFadeAway;
               textColor.a = 1 - completion;
               anchorPosition.y = completion * 20;

               TextDisplay.color = textColor;
               textTransform.anchoredPosition = anchorPosition;
          }
          gameObject.SetActive(false);

     }
}
