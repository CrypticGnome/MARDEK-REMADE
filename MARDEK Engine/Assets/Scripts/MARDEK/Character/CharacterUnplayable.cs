using MARDEK.CharacterSystem;
using MARDEK.Inventory;
using System;
using UnityEngine;
namespace MARDEK.CharacterSystem
{
     [CreateAssetMenu(fileName = "CharacterUnplayable", menuName = "Scriptable Objects/CharacterUnplayable")]
     public class CharacterUnplayable : Character
     {
          public ItemDrop[] Drops;

          [Serializable]
          public class ItemDrop
          {
               public Item Item;
               public float Chance;
          }
     }

}