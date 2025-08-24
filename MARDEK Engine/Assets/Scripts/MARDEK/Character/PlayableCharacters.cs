using MARDEK.CharacterSystem;
using MARDEK.Core;
using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.Progress
{
     [CreateAssetMenu(fileName = "UnlockedCharacters", menuName = "Scriptable Objects/UnlockedCharacters")]
     public class PlayableCharacters : AddressableScriptableObject
     {
          public PartySO Party;
          public List<Character> BenchedCharacters;
          public List<Character> UnlockedCharacters;
     }
}