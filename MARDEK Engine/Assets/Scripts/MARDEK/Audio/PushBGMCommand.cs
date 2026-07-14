using MARDEK.Event;
using UnityEngine;

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