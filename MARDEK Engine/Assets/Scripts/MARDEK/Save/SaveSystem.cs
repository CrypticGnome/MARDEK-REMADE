using System;
using System.Collections.Generic;
using MARDEK.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Guid = System.Guid;

#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace MARDEK.Save
{
	public class SaveSystem : MonoBehaviour
	{
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SyncDB();
#endif

		public static string persistentPath
		{
			get
			{
				string path;

#if UNITY_WEBGL
                path = System.IO.Path.Combine("/idbfs", Application.productName);
#else
				path = Application.persistentDataPath;
#endif

				if (System.IO.Directory.Exists(path) == false)
					System.IO.Directory.CreateDirectory(path);
				return path;
			}
		}
		static readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
		{
			ContractResolver = new SaveContractResolver(),
			Converters = { new GuidReferenceConverter() },
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			// replace collections/objects on load instead of merging into inspector defaults
			ObjectCreationHandling = ObjectCreationHandling.Replace,
			Formatting = Formatting.Indented,
		};
		static readonly JsonSerializer serializer = JsonSerializer.Create(serializerSettings);
		public delegate void SaveCallback();
		public static event SaveCallback OnBeforeSave = delegate { };

		static SaveState internalSaveState;
		static string currentSaveFileName;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void Initialization()
		{
			internalSaveState = new SaveState();
		}

		public static void SaveObject(IAddressableGuid addressable)
		{
			if (Application.isPlaying == false)
				throw new Exception("Don't Save while outside playmode");
			internalSaveState.SaveObject(addressable, serializer);

		}
		public static bool LoadObject(IAddressableGuid addressable, SaveState saveState = null)
		{
			if (Application.isPlaying == false)
				throw new Exception("Don't Load while outside playmode");
			if (saveState == null)
				return internalSaveState.LoadObject(addressable, serializer);
			else
				return saveState.LoadObject(addressable, serializer);
		}

		public static void SaveToFile(string fileName)
		{
			PlayerPrefs.SetString("lastSavedFile", fileName);

			OnBeforeSave.Invoke();
			string json = JsonConvert.SerializeObject(internalSaveState.addressableState, serializerSettings);
			string filePath = System.IO.Path.Combine(persistentPath, $"{fileName}.json");
			System.IO.File.WriteAllText(filePath, json);

#if UNITY_WEBGL && !UNITY_EDITOR
            //flush our changes to IndexedDB
            SyncDB();
#endif

			Debug.Log($"Game file saved to {filePath}");
		}
		public static void CallGameFileLoaderScene(string fileName)
		{
			currentSaveFileName = fileName;
			PlayerPrefs.SetString("lastLoadedFile", currentSaveFileName);
			SceneManager.LoadScene(1);
		}

		public void LoadCurrentSaveFileNameIntoInternalSaveState()
		{
			internalSaveState = GetSaveStateFromFile(currentSaveFileName);
		}

		public static SaveState GetSaveStateFromFile(string fileName)
		{
			string filePath = System.IO.Path.Combine(persistentPath, $"{fileName}.json");

			SaveState resultSaveState = new SaveState();
			string json = System.IO.File.ReadAllText(filePath);
			var addressableState = JsonConvert.DeserializeObject<Dictionary<Guid, JObject>>(json, serializerSettings);
			if (addressableState != null)
				resultSaveState.addressableState = addressableState;
			return resultSaveState;
		}
	}
}
