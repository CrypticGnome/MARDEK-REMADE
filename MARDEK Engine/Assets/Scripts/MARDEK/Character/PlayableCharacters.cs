using System.Collections.Generic;
using MARDEK.CharacterSystem;
using MARDEK.Core;
using UnityEngine;

namespace MARDEK.Progress
{
	[CreateAssetMenu(fileName = "UnlockedCharacters", menuName = "MARDEK/Progress/UnlockedCharacters")]
	public class PlayableCharacters : AddressableScriptableObject
	{
		public PartySO Party;
		public List<Character> BenchedCharacters;
		public List<Character> UnlockedCharacters;
	}
}