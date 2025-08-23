using MARDEK.CharacterSystem;
using MARDEK.Core;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PartySO", menuName = "MARDEK/Singeltons/Party")]
public class PartySO : AddressableScriptableObject
{
     public List<Character> OnFieldCharacters;
     public List<Character> RequiredSetup;


}
