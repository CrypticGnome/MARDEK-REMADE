using MARDEK.Battle;
using UnityEngine;

namespace MARDEK
{
    public class BattlefieldLoader : MonoBehaviour
    {
          [SerializeField] PrefabSO currentBackground;
          [SerializeField] GameObject fallbackBackground;
          [SerializeField] Transform battlefieldBackgroundParent;
        void Start()
        {
               GameObject instantiatedObject;
               if (currentBackground == null || currentBackground.CurrentPrefab == null) instantiatedObject = fallbackBackground;
               else instantiatedObject = currentBackground.CurrentPrefab;

               Instantiate(instantiatedObject, battlefieldBackgroundParent);
        }

    }
}
