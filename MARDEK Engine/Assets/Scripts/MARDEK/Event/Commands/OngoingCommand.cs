using UnityEngine;

namespace MARDEK.Event
{
    public abstract class OngoingCommand : Command
    {
        [SerializeField] protected bool _waitExcecutionEnd = true;
        public bool WaitForExecutionEnd { get { return _waitExcecutionEnd; } }
        public abstract bool IsOngoing();
        public virtual void UpdateCommand() { }
    }
}