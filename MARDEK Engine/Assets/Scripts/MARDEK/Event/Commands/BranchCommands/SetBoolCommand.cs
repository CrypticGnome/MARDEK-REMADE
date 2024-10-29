using MARDEK.Save;
using UnityEngine;

namespace MARDEK.Event
{
    public class SetBoolCommand : Command
    {
        [SerializeField] LocalSwitchBool boolObject;
        [SerializeField] bool setValue;

        public override void Trigger()
        {

               boolObject.SetBoolValue(setValue);

        }
    }
}