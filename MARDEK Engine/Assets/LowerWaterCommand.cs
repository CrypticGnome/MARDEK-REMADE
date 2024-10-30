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
          [SerializeField] bool raiseOnTrue;
          [SerializeField] LocalSwitchBool boolVariable;
          [SerializeField] bool defaultValue;
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
          private void Start()
          {
               if (boolVariable == null)
                    return;
               if (boolVariable.GetBoolValue() == defaultValue)
                    Raised();
               else
                    Lowered();
          }

          public override void Trigger()
          {
               if (boolVariable.GetBoolValue() == raiseOnTrue)
                    StartCoroutine(Raise());
               else
                    StartCoroutine(Lower());
          }
          IEnumerator Lower()
          {
               Color color = tileMap.color;
               Debug.Log("LOWERING WATER");
               while (lowered < 1)
               {
                    lowered += Time.deltaTime / duration;
                    transform.localPosition = loweredPosition * lowered;
                    color = tileMap.color;
                    color.a = Mathf.Lerp(raisedTransparency, loweredTransparency, lowered)/255;
                    tileMap.color = color;
                    yield return null;
               }

               Lowered();
          }
          IEnumerator Raise()
          {
               Color color = tileMap.color;
               Debug.Log("RAISING WATER");

               while (lowered > 0)
               {
                    lowered -= Time.deltaTime / duration;
                    transform.localPosition = loweredPosition * lowered;
                    color.a = Mathf.Lerp(raisedTransparency, loweredTransparency, lowered)/255;
                    tileMap.color = color;
                    yield return null;
               }
               Raised();
          }
          public override bool IsOngoing()
          {
               return lowered != 0 && lowered != 1;
          }
          void Lowered()
          {
               Color color = tileMap.color;
               lowered = 1;
               transform.localPosition = loweredPosition;
               color.a = loweredTransparency / 255;
               tileMap.color = color;
          }
          void Raised()
          {
               Color color = tileMap.color;
               lowered = 0;
               transform.localPosition = Vector3.zero;
               color.a = raisedTransparency / 255;
               tileMap.color = color;
          }
     }
}
