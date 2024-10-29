using MARDEK.Event;
using MARDEK.Save;
using UnityEngine;

namespace MARDEK.Event
{
    public class CycleSpriteRendererCommand : Command
    {
          SpriteRenderer spriteRenderer;
          [SerializeField] Sprite[] sprites;
          [SerializeField]int startIndex;
          int index;
#if UNITY_EDITOR
          private void OnValidate()
          {
               if (spriteRenderer is null)
               {

                    if (!TryGetComponent(out spriteRenderer))
                    {
                         gameObject.AddComponent<SpriteRenderer>();
                    }
               }
               spriteRenderer.sprite = sprites[startIndex];
          }
#endif
          private void Start()
          {
               index = startIndex;
          }
          public override void Trigger()
          {
               index++;
               if (index >= sprites.Length)
                    index = 0;
               spriteRenderer.sprite = sprites[index];
          }
     }
}
