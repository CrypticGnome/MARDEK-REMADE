using UnityEngine;

namespace MARDEK.Battle
{
     [CreateAssetMenu(fileName = "New Prefab SO", menuName ="MARDEK/Core/Prefab Storage")]
     public class PrefabSO : ScriptableObject
     {
          public GameObject CurrentPrefab;
     }
}