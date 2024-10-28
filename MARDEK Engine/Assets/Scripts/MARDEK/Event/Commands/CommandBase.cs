using UnityEngine;

namespace MARDEK.Event
{
    public abstract class Command : MonoBehaviour
    {
        public abstract void Trigger();
    }
}