using UnityEngine;
using UnityEngine.UIElements;

namespace MARDEK.Battle
{
    public class BattleModelComponent : MonoBehaviour
    {
          [SerializeField] AnimationClip idle, moveto, strike, jumpback, hit, die, dead, spellcast, useItem, victory;
          [SerializeField, HideInInspector] new UnityEngine.Animation animation;
          [SerializeField, HideInInspector] new Transform transform;
          [SerializeField] Transform crystalPointerGoToPosition;
          [SerializeField] DamageDisplay damageDisplay;
          public DamageDisplay DamageDisplay => damageDisplay;
          public Transform CrystalPointerGoToPosition => crystalPointerGoToPosition;
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
               animation.wrapMode = WrapMode.Loop;

               var layer = SortingLayer.NameToID($"BattleModel {(int)transform.position.z}");
               foreach (var r in GetComponentsInChildren<SpriteRenderer>())
                    r.sortingLayerID = layer;
          }



     }

     public enum AttackType
    {
          Melee,
          Spellcast,
          Breath,
    }
}
