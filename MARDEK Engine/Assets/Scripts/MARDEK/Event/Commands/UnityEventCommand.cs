using MARDEK.Core.LevelDesign;
using MARDEK.Save;
using UnityEngine;
using UnityEngine.Events;

namespace MARDEK.Event
{
    public class UnityEventCommand : Command
    {
          [SerializeField] UnityEvent _event;


          public override void Trigger() => _event.Invoke();

    }
}