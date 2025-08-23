using MARDEK.Core;
using UnityEngine;

namespace MARDEK
{
     [CreateAssetMenu(fileName = "ScriptableBool", menuName = "MARDEK/Level Design/Bool")]
     public class ScriptableBool : AddressableScriptableObject
     {
          public bool Value;

     }
}