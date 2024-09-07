using UnityEngine;

public class BattleModel : MonoBehaviour
{
     [SerializeField] string nameFrom5118;
     [SerializeField] int spriteID;
     [SerializeField] string skin;

     private void Awake()
     {
          ApplySkin();
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

     public void Create(string name, int spriteID, string skin)
     {
          nameFrom5118 = name;
          this.spriteID = spriteID;
          this.skin = skin;
     }

     [ContextMenu("Import")]
     public void Import()
     {
          var obj = SWFSpriteImporter.CreateOrInstantiateByID(spriteID, transform);
          foreach (var shape in obj.GetComponentsInChildren<SWFShape>())
               shape.CalculateSortingOrderAndColor();
     }

     [ContextMenu("Apply Skin")]
     public void ApplySkin()
     {
          SWFSprite[] sprites = GetComponentsInChildren<SWFSprite>();
          for (int index = 0; index < sprites.Length; index++)
          {
               sprites[index].PlayAnimationByLabel(skin);
          }
     }
}