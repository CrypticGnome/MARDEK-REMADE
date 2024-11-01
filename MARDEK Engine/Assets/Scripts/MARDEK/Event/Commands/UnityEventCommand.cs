using MARDEK.Save;
using UnityEngine;
using UnityEngine.Events;

namespace MARDEK.Event
{
    public class UnityEventCommand : Command
    {
          [SerializeField] UnityEvent _event = default;
          [SerializeField] LocalSwitchBool @bool;
          [SerializeField] bool activationValue;
          
          private void Start()
          {
               if (@bool != null) 
                    if (@bool.GetBoolValue() == activationValue)
                         Trigger();
          }

          public override void Trigger()
          {
               _event.Invoke();
          }
    }
}