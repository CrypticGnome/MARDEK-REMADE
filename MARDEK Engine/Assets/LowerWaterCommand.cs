using MARDEK.Event;
using MARDEK.Save;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MARDEK
{
    public class LowerWaterCommand : OngoingCommand
    {
          [SerializeField] Tilemap tileMap;
          [SerializeField] LocalSwitchBool raise;
          [SerializeField] float duration;
          [SerializeField] Vector3 loweredPosition;
          [SerializeField] float raisedTransparency, loweredTransparency;

          float lowered;
#if UNITY_EDITOR
          private void OnValidate()
          {
               if (tileMap is null)
               {
                    if (!TryGetComponent(out tileMap))
                    {
                         gameObject.AddComponent<SpriteRenderer>();
                    }
               }
               Color color = tileMap.color;
               color.a = raisedTransparency / 255;
               tileMap.color = color;
          }
#endif


          public override void Trigger()
          {
               if (raise.GetBoolValue())
                    StartCoroutine(Raise());
               else
                    StartCoroutine(Lower());
          }
          IEnumerator Lower()
          {
               Color color = tileMap.color;

               while (lowered < 1)
               {
                    lowered += Time.deltaTime / duration;
                    transform.localPosition = loweredPosition * lowered;
                    color = tileMap.color;
                    color.a = Mathf.Lerp(raisedTransparency, loweredTransparency, lowered)/255;
                    tileMap.color = color;
                    yield return null;
               }

               lowered = 1;
               transform.localPosition =  loweredPosition;
               color.a = loweredTransparency / 255;
               tileMap.color = color;
          }
          IEnumerator Raise()
          {
               Color color = tileMap.color;
               while (lowered > 0)
               {
                    lowered -= Time.deltaTime / duration;
                    transform.localPosition = loweredPosition * lowered;
                    color.a = Mathf.Lerp(raisedTransparency, loweredTransparency, lowered)/255;
                    tileMap.color = color;
                    yield return null;
               }
               lowered = 0;
               transform.localPosition = Vector3.zero;
               color.a = raisedTransparency/255;
               tileMap.color = color;
          }
          public override bool IsOngoing()
          {
               return lowered != 0 && lowered != 1;
          }
     }
}
