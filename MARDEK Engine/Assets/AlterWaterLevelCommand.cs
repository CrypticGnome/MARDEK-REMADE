using MARDEK.Event;
using MARDEK.Save;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MARDEK
{
    public class AlterWaterLevelCommand : OngoingCommand
    {
          [SerializeField] Tilemap raisedTileMap, loweredTileMap;
          [SerializeField] ConditionComponent raisedCondition;
          [SerializeField] float duration;
          [SerializeField] Vector3 loweredPosition;
          [SerializeField] float raisedTransparency, loweredTransparency;

          float lowered;

          private void Start()
          {
               if (raisedCondition.Condition.Value)
                    Raised();
               else
                    Lowered();
          }

          public override void Trigger()
          {
               if (raisedCondition.Condition.Value)
                    StartCoroutine(Raise());
               else
                    StartCoroutine(Lower());
          }
          IEnumerator Lower()
          {
               raisedTileMap.gameObject.SetActive(true);
               loweredTileMap.gameObject.SetActive(false);
               Color color = raisedTileMap.color;
               Debug.Log("LOWERING WATER");
               while (lowered < 1)
               {
                    lowered += Time.deltaTime / duration;
                    transform.localPosition = loweredPosition * lowered;
                    color = raisedTileMap.color;
                    color.a = Mathf.Lerp(raisedTransparency, loweredTransparency, lowered)/255;
                    raisedTileMap.color = color;
                    yield return null;
               }

               Lowered();
          }
          IEnumerator Raise()
          {
               raisedTileMap.gameObject.SetActive(true);
               loweredTileMap.gameObject.SetActive(false);

               Color color = raisedTileMap.color;
               Debug.Log("RAISING WATER");

               while (lowered > 0)
               {
                    lowered -= Time.deltaTime / duration;
                    transform.localPosition = loweredPosition * lowered;
                    color.a = Mathf.Lerp(raisedTransparency, loweredTransparency, lowered)/255;
                    raisedTileMap.color = color;
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
               raisedTileMap.gameObject.SetActive(false);
               loweredTileMap.gameObject.SetActive(true);
               Color color = raisedTileMap.color;
               lowered = 1;
               transform.localPosition = loweredPosition;
               color.a = loweredTransparency / 255;
               raisedTileMap.color = color;
          }
          void Raised()
          {
               raisedTileMap.gameObject.SetActive(true);
               loweredTileMap.gameObject.SetActive(false);
               Color color = raisedTileMap.color;
               lowered = 0;
               transform.localPosition = Vector3.zero;
               color.a = raisedTransparency / 255;
               raisedTileMap.color = color;
          }
     }
}
