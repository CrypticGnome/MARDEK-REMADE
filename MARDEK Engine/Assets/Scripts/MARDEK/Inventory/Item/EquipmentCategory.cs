using MARDEK.Core;
using UnityEngine;

namespace MARDEK.Inventory
{
    [CreateAssetMenu(menuName = "MARDEK/Inventory/EquipmentCategory")]
    public class EquipmentCategory : ScriptableObject
    {
        [SerializeField] EquipmentSlotType _slot;
        [SerializeField] string _classification;

        public EquipmentSlotType slot { get { return _slot; } }

          public Color color { 
               get 
               {
                    Debug.LogWarning("WIP, colours not properly implemented yet");
                    switch (slot) {
                         case EquipmentSlotType.MainHand:
                              return Color.red;
                         case EquipmentSlotType.OffHand:
                              return Color.red;
                         case EquipmentSlotType.Head:
                              return Color.green;
                         case EquipmentSlotType.Body:
                              return Color.green;
                         case EquipmentSlotType.Accessory:
                              return Color.blue;
                         default:
                              Debug.LogAssertion(slot + " does not have an associated colour");
                              return Color.black;
                    }
               } 
        }

        public string classification { get { return _classification; } }
          public enum EquipmentSlotType
          {
               MainHand,
               OffHand,
               Head,
               Body,
               Accessory
          }
    }
}