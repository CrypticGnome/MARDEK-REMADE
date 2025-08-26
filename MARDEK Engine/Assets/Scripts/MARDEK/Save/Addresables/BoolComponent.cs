using UnityEngine;
using MARDEK.Event;

namespace MARDEK.Save
{
    [System.Serializable]
    public class BoolComponent : AddressableMonoBehaviour
    {
        [SerializeField] protected bool value = false;
#if UNITY_EDITOR
          [SerializeField, Multiline] string Description;
        #endif
          public bool Value { get { return value; } set { this.value = value; } }

          private void OnDestroy()
          {
               Save();
          }
     }
}