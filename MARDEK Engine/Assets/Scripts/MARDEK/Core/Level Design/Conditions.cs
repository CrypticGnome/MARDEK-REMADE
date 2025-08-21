using System;
using UnityEngine;

namespace MARDEK.Core.LevelDesign
{
     [Serializable]
     public class LocalBoolCondition : ICondition
     {
          public bool Condition => BoolCondition.Condition;

          public LocalBoolCondition BoolCondition;
     }
}