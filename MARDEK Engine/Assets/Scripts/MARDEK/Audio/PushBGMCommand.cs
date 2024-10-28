using UnityEngine;
using MARDEK.Event;

namespace MARDEK.Audio
{
    public class PushBGMCommand : Command
    {
        [SerializeField] Music music;

        public override void Trigger()
        {
            AudioManager.PushMusic(music);
        }
    }
}