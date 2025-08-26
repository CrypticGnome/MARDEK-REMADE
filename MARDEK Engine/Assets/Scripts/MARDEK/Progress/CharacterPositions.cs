using MARDEK.Core;
using UnityEngine;

namespace MARDEK.Progress
{
     [CreateAssetMenu(fileName = "CharacterPositions", menuName = "MARDEK/Progress/CharacterPositions")]
     public class CharacterPositions : AddressableScriptableObject
     {
          public Vector2[] Positions;
          public MoveDirection[] Directions;
     }
}