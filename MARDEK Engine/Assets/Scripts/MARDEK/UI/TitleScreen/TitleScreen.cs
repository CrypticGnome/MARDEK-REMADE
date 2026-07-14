using MARDEK.Save;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MARDEK.UI
{
	public class TitleScreen : MonoBehaviour
	{
		[SerializeField] GeneralProgressData progressData;
		[SerializeField] InputField gameName;
		[SerializeField] UnityEvent startGame;

		public void TryStartNewGame()
		{
			if (ValidateName(gameName.text))
			{
				progressData.GameName = gameName.text;
				startGame.Invoke();
			}
		}

		bool ValidateName(string name)
		{
			return !string.IsNullOrEmpty(name);
		}
	}
}
