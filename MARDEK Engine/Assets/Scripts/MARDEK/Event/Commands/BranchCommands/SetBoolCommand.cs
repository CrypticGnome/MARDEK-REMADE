using MARDEK.Save;
using UnityEngine;

namespace MARDEK.Event
{
    public class SetBoolCommand : Command
    {
        [SerializeField] BoolComponent boolObject;
        [SerializeField] bool setValue;

        public override void Trigger()
        {
               boolObject.Value = setValue;
        }
    }
}