using UnityEngine;
using MARDEK;

namespace MARDEK 
{
     public class BattleModel : MonoBehaviour
     {
          [SerializeField] new Animation animation;
          [SerializeField] AnimationClip idle, moveTo, strike, jumpback, hurt, die, dead, spellcast, useItem, victory;
          private void Awake()
          {
          }

          public void SetBattlePosition(Vector3 position)
          {
               gameObject.transform.position = position;
               if (position.x < 0)
                    gameObject.transform.localScale = new Vector3(-1, 1, 1);
               var layer = SortingLayer.NameToID($"BattleModel {(int)position.z}");
               foreach (var r in GetComponentsInChildren<SpriteRenderer>(includeInactive: true))
                    r.sortingLayerID = layer;
          }


          public void PlayAnimation(AnimationType animationType)
          {
               switch (animationType)
               {
                    default:
                    case AnimationType.idle: animation.clip = idle; break;
                    case AnimationType.moveTo: animation.clip = moveTo; break;
                    case AnimationType.strike: animation.clip = strike; break;
                    case AnimationType.jumpback: animation.clip = jumpback; break;
                    case AnimationType.hurt: animation.clip = hurt; break;
                    case AnimationType.die: animation.clip = die; break;
                    case AnimationType.dead: animation.clip = dead; break;
                    case AnimationType.spellcast: animation.clip = spellcast; break;
                    case AnimationType.useItem: animation.clip = useItem; break;
                    case AnimationType.victory: animation.clip = victory; break;
               }
               animation.Play();
          }
          public enum AnimationType
         {
               idle,
               moveTo,
               strike,
               jumpback,
               hurt,
               die, dead,
               spellcast, useItem, victory
         }
     }
     
}