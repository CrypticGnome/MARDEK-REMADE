using UnityEngine;
using MARDEK.Progress;
namespace MARDEK.Inventory
{
     [CreateAssetMenu(fileName = "InventorySO", menuName = "MARDEK/Progress/InventorySO")]
     public class InventorySO : ScriptableObject
     {
          public PlotItems PlotItems;
          public int Money;
          public PlayableCharacters Characters;
     }

}