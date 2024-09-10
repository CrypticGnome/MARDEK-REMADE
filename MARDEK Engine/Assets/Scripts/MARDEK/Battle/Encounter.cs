using MARDEK.CharacterSystem;
using MARDEK.Inventory;
using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.Battle
{
     [CreateAssetMenu(menuName ="MARDEK/Battle/Encounter")]
     public class Encounter : ScriptableObject
     {
          public EnemyWithLevelRange[] Enemies;
          public Item[] UniqueRewards;

          [System.Serializable]
          public class EnemyWithLevelRange
          {
               public Character enemy;
               public int minLevel = 0;
               public int maxLevel = 0;
          }
     }
}