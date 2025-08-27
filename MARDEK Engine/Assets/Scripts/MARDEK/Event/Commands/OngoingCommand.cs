using MARDEK.Core;
using System.Collections;
using UnityEngine;

namespace MARDEK.Event
{
    public abstract class OngoingCommand : Command
    {
        [SerializeField] protected bool lockPlayerActions = true;
        [SerializeField] protected bool runParallel = false;

          public bool LockPlayerActions { get { return lockPlayerActions; } }
          public bool RunParallel { get { return runParallel; } }

          public abstract bool IsOngoing();
        public virtual void UpdateCommand() { }
        public virtual IEnumerator TriggerAsync()
        {
               Trigger();
               if (LockPlayerActions) PlayerLocks.EventSystemLock++;
               yield return new WaitUntil(() => !IsOngoing());
               if (LockPlayerActions) PlayerLocks.EventSystemLock--;
          }


     }
}