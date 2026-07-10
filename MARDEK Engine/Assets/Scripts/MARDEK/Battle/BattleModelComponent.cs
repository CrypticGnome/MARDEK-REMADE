using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace MARDEK.Battle
{
    public class BattleModelComponent : MonoBehaviour
    {
          [SerializeField] AnimationClip idle, moveto, strike, jumpback, hurt, die, dead, spellcast, useItem, victory;
          [SerializeField, HideInInspector] new UnityEngine.Animation animation;
          [SerializeField, HideInInspector] new Transform transform;
          [SerializeField] Transform crystalPointerGoToPosition;
          [SerializeField] Transform strikePoint;
          [SerializeField] Transform hitPoint;
          [SerializeField] float moveToDuration = 0.5f;
          [SerializeField] DamageDisplay damageDisplay;
          public DamageDisplay DamageDisplay => damageDisplay;
          public Transform CrystalPointerGoToPosition => crystalPointerGoToPosition;
          public Transform StrikePoint => strikePoint;
          public Transform HitPoint => hitPoint;
          private void OnValidate()
          {
               if (transform == null)
                    transform = GetComponent<Transform>();
               if (animation == null)
                    animation = GetComponentInChildren<UnityEngine.Animation>();
               if (damageDisplay == null)
                    damageDisplay = GetComponentInChildren<DamageDisplay>();
          }
          // Start is called once before the first execution of Update after the MonoBehaviour is created
          void Start()
        {
               animation.clip = idle;
               animation.Play(idle.name);
               AnimationState state = animation[idle.name];
               state.time = Random.Range(0f, state.length);

               animation.wrapMode = WrapMode.Loop;

               var layer = SortingLayer.NameToID($"BattleModel {(int)transform.position.z}");
               foreach (var r in GetComponentsInChildren<SpriteRenderer>())
                    r.sortingLayerID = layer;
          }

          public void PlayAnimation(BattleAnimationType animType)
          {
               switch (animType)
               {
                    default:
                    case BattleAnimationType.Idle:
                    {
                         animation.clip = idle;
                         animation.Play(idle.name);
                         break;
                    }
                    case BattleAnimationType.MoveTo:
                    {
                         StartCoroutine(PlayClipAndReturnToIdle(moveto));
                         break;
                    }
                    case BattleAnimationType.Strike:
                    {
                         StartCoroutine(PlayClipAndReturnToIdle(strike));
                         break;
                    }
                    case BattleAnimationType.JumpBack:
                    {
                         StartCoroutine(PlayClipAndReturnToIdle(jumpback));
                         break;
                    }
                    case BattleAnimationType.Hurt:
                    {
                         StartCoroutine(PlayClipAndReturnToIdle(hurt));
                         break;
                    }
                    case BattleAnimationType.Die:
                    {
                         animation.clip = die;
                         animation.Play(die.name);
                         animation.wrapMode = WrapMode.Once;
                         break;
                    }
                    case BattleAnimationType.Dead:
                    {
                         animation.clip = dead;
                         animation.Play(dead.name);
                         animation.wrapMode = WrapMode.Loop;
                         break;
                    }
                    case BattleAnimationType.Spellcast:
                    {
                         StartCoroutine(PlayClipAndReturnToIdle(spellcast));
                         break;
                    }
                    case BattleAnimationType.UseItem:
                    {
                         StartCoroutine(PlayClipAndReturnToIdle(useItem));
                         break;
                    }
                    case BattleAnimationType.Victory:
                    {
                         animation.clip = victory;
                         animation.Play(victory.name);
                         animation.wrapMode = WrapMode.Loop;
                         break;
                    }
               }

               IEnumerator PlayClipAndReturnToIdle(AnimationClip clip)
               {
                    animation.clip = clip;
                    animation.Play(clip.name);
                    yield return new WaitForSeconds(clip.length);
                    animation.clip = idle;
                    animation.Play(idle.name);
               }
          }

          /// <summary>
          /// Runs the full melee approach: MoveTo while travelling until this model's
          /// Strike Point overlaps the target's Hit Point, Strike, then JumpBack while
          /// returning to the idle position.
          /// </summary>
          public IEnumerator PlayMeleeSequence(BattleModelComponent target)
          {
            Vector3 idlePosition = transform.position;

            Vector3 attackPosition = idlePosition;
            if (target is not null && strikePoint is not null && target.hitPoint is not null)
            {
                attackPosition = idlePosition + (target.hitPoint.position - strikePoint.position);
            }

            yield return MoveWithClip(moveto, idlePosition, attackPosition, moveToDuration);
               animation.clip = strike;
               animation.Play(strike.name);
               yield return new WaitForSeconds(strike.length);
               yield return MoveWithClip(jumpback, attackPosition, idlePosition, jumpback.length);
               animation.clip = idle;
               animation.Play(idle.name);

               IEnumerator MoveWithClip(AnimationClip clip, Vector3 from, Vector3 to, float duration)
               {
                    animation.clip = clip;
                    animation.Play(clip.name);
                    for (float t = 0; t < duration; t += Time.deltaTime)
                    {
                         transform.position = Vector3.Lerp(from, to, t / duration);
                         yield return null;
                    }
                    transform.position = to;
               }
          }

     }
     public enum BattleAnimationType
     {
          Idle,
          MoveTo,
          Strike,
          JumpBack,
          Hurt,
          Die,
          Dead,
          Spellcast,
          UseItem,
          Victory
     }
     public enum AttackType
    {
          Melee,
          Spellcast,
          Breath,
    }
}
