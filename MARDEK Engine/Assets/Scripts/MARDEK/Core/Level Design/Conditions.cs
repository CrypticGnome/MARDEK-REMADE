using MARDEK.Save;
using System;
using UnityEngine;

namespace MARDEK.Core.LevelDesign
{
     [Serializable]
     public class LocalBoolCondition : Condition
     {
          public override bool Value => BoolCondition.GetBoolValue();

          public LocalSwitchBool BoolCondition;
     }
}