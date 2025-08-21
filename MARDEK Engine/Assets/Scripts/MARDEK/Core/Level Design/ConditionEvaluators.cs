using System;
using UnityEngine;

namespace MARDEK.Core.LevelDesign
{
     [Serializable]
     public class ANDCondition : ICondition
     {
          public bool Condition => ConditionA.Condition && ConditionB.Condition; 
          public ICondition ConditionA;
          public ICondition ConditionB;
     }

     [Serializable]
     public class ORCondition : ICondition
     {
          public bool Condition => ConditionA.Condition || ConditionB.Condition;
          public ICondition ConditionA;
          public ICondition ConditionB;
     }

     [Serializable]
     public class NOTCondition : ICondition
     {
          public bool Condition => !InCondition.Condition;
          public ICondition InCondition;
     }

     [Serializable]
     public class XORCondition : ICondition
     {
          public bool Condition => ConditionA.Condition ^ ConditionB.Condition;
          public ICondition ConditionA;
          public ICondition ConditionB;
     }
}