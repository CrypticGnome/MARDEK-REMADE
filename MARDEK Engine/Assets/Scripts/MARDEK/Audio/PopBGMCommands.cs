using MARDEK.Event;

namespace MARDEK.Audio
{
    public class PopBGMCommands : Command
    {
        public override void Trigger()
        {
            AudioManager.PopMusic();
        }
    }
}