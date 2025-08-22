using MARDEK.Event;
using UnityEngine;

public class WaitCommand : OngoingCommand
{
     [SerializeField] float waitTime;
     float endWaitTime;
     public override bool IsOngoing()
     {
          return Time.time < endWaitTime;
     }

     public override void Trigger()
     {
          endWaitTime = Time.time + waitTime;
     }

}
