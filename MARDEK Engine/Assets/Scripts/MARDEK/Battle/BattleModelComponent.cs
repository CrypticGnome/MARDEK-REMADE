using UnityEngine;
using UnityEngine.UIElements;

namespace MARDEK.Battle
{
    public class BattleModelComponent : MonoBehaviour
    {
          [SerializeField] AnimationClip idle, moveto, strike, jumpback, hit, die, dead, spellcast, useItem, victory;
          [SerializeField] new UnityEngine.Animation animation;
          [SerializeField] new Transform transform;
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
