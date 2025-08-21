using UnityEngine;
using MARDEK.Event;

namespace MARDEK.Save
{
    [System.Serializable]
    public class LocalSwitchBool : AddressableMonoBehaviour
    {
        [SerializeField] protected bool value = false;
#if UNITY_EDITOR
          [SerializeField, Multiline] string Description;
        #endif
        public bool GetBoolValue()
        {
            return value;
        }

        public virtual void SetBoolValue(bool setValue)
        {
            value = setValue;
        }
    }
}