using MARDEK.Event;
using MARDEK.Save;
using UnityEngine;

namespace MARDEK.Event
{
     public class SwitchBoolCommand : Command
     {

          [SerializeField] BoolComponent boolObject;

          public override void Trigger()
          {
               boolObject.Value = !boolObject.Value;
          }

     }
}