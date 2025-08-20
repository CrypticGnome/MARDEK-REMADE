using UnityEngine;

namespace MARDEK.Event
{
    public abstract class OngoingCommand : Command
    {
        [SerializeField] protected bool lockPlayerActions = true;
        public bool LockPlayerActions { get { return lockPlayerActions; } }
        public abstract bool IsOngoing();
        public virtual void UpdateCommand() { }
    }
}