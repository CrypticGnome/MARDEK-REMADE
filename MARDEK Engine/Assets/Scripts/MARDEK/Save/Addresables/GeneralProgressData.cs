using System;
using MARDEK.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MARDEK.Save
{
	public class GeneralProgressData : AddressableMonoBehaviour
	{

		[SerializeField] public string currentScene = default;
		[SerializeField] string _gameName = string.Empty;
		[SerializeField] public string SceneName = string.Empty;
		[SerializeField] public DateTime SavedTime = new DateTime();

		public string GameName
		{
			get
			{
				return _gameName;
			}
			set
			{
				if (string.IsNullOrEmpty(_gameName))
					_gameName = value;
				return;
			}
		}

		public override void Save()
		{
			Scene scene = SceneManager.GetActiveScene();
			currentScene = scene.path;

			SceneName = SceneInfo.CurrentSceneInfoDisplayName;
			SavedTime = DateTime.Now;

			base.Save();
		}

		public void LoadScene()
		{
			if (string.IsNullOrEmpty(currentScene))
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			else
				SceneManager.LoadScene(currentScene);
		}
	}
}
