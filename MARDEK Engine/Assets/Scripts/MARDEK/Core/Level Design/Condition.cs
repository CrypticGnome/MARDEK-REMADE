using System;
using UnityEngine;

namespace MARDEK.Core.LevelDesign
{
     [Serializable]
     public abstract class Condition
     {
          public abstract bool Value {get;}
     }
}