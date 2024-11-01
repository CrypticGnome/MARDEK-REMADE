using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.Animation
{
    using Event;
    public class StopSpriteAnimationCommand : Command
    {
        [SerializeField] SpriteAnimator targetAnimator;
        [SerializeField] bool sendStopRate = false;
        [SerializeField, Range(0f, 1f)] float animationStopRate = 1f;

        public override void Trigger()
        {
            if (targetAnimator == null)
                targetAnimator = SpriteAnimator.PlayerSpriteAnimator;
            if (sendStopRate)
                targetAnimator.StopCurrentAnimation(animationStopRate);
            else
                targetAnimator.StopCurrentAnimation();
        }
    }
}