using MARDEK.Core;
using UnityEngine;

namespace MARDEK.Event
{
    public class SetPositionCommand : Command
    {
          [SerializeField] Transform setTransform;
          [SerializeField] Vector2 absolouteOffset;
          [SerializeField] Vector2 relativeOffset;
          [SerializeField] bool useRelativeOffset = true;

          public override void Trigger()
          {
               if (!useRelativeOffset)
                    setTransform.Set2DPosition(absolouteOffset);
               else
                    setTransform.Set2DPosition((Vector2)setTransform.position + relativeOffset);
          }
     }
}
