using UnityEngine;
using MARDEK.Event;

namespace MARDEK.Audio
{
    public class PlaySFXCommand : Command
    {
        [SerializeField] SoundEffect sound;

        public override void Trigger()
        {
            AudioManager.PlaySoundEffect(sound);
        }
    }
}
