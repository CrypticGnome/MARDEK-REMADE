using MARDEK.CharacterSystem;
using MARDEK.Core;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MARDEK.Progress
{
     [CreateAssetMenu(fileName = "PartySO", menuName = "MARDEK/Progress/Party")]
     public class PartySO : AddressableScriptableObject, IEnumerable<Character>
     {
          public List<Character> OnFieldCharacters;
          public List<Character> RequiredSetup;
          public int Count => OnFieldCharacters.Count;

          public Character Current => throw new System.NotImplementedException();

          public IEnumerator<Character> GetEnumerator() => OnFieldCharacters.GetEnumerator();
          public Character this[int index] => OnFieldCharacters[index];

          IEnumerator IEnumerable.GetEnumerator()
          {
               return GetEnumerator();
          }
          public static implicit operator List<Character>(PartySO s) => s.OnFieldCharacters;
     }
}