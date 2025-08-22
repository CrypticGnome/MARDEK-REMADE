using System;
using UnityEngine;

namespace MARDEK.Core.LevelDesign
{
     [Serializable]
     public class ANDCondition : Condition
     {
          public override bool Value => ConditionA.Value && ConditionB.Value;

          [SerializeReference, SubclassSelector] public Condition ConditionA;
          [SerializeReference, SubclassSelector] public Condition ConditionB;
          
     }

     [Serializable]
     public class ORCondition : Condition
     {
          public override bool Value => ConditionA.Value || ConditionB.Value;
          [SerializeReference, SubclassSelector] public Condition ConditionA;
          [SerializeReference, SubclassSelector] public Condition ConditionB;
     }

     [Serializable]
     public class NOTCondition : Condition
     {
          public override bool Value => !InCondition.Value;
          [SerializeReference, SubclassSelector] public Condition InCondition;
     }

     [Serializable]
     public class XORCondition : Condition
     {
          public override bool Value => ConditionA.Value ^ ConditionB.Value;
          [SerializeReference, SubclassSelector] public Condition ConditionA;
          [SerializeReference, SubclassSelector] public Condition ConditionB;
     }
}