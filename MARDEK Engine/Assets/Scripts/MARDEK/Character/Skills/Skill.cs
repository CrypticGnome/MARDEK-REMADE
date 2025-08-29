using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MARDEK.Stats;
using MARDEK.Core;

namespace MARDEK.Skill
{
     public class Skill : AddressableScriptableObject
     {
          [field: SerializeField] public int PointsRequiredToMaster { get; private set; }
          [field: SerializeField] public string DisplayName { get; private set; }
          [field: SerializeField] public string Description { get; private set; }
     }
}