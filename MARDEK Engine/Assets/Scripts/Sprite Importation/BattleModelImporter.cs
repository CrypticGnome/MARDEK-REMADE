using MARDEK;
using UnityEngine;

[RequireComponent(typeof(BattleModel))]
public class BattleModelImporter : MonoBehaviour
{
     [SerializeField] string nameFrom5118;
     [SerializeField] int spriteID;
     [SerializeField] string skin;

      // Start is called once before the first execution of Update after the MonoBehaviour is created
      void Start()
     {
          ApplySkin();
     }
     
     // Update is called once per frame
     void Update()
     {
         
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
     public void Create(string name, int spriteID, string skin)
     {
          nameFrom5118 = name;
          this.spriteID = spriteID;
          this.skin = skin;
     }
}
