using System;
using MARDEK.Core;
using UnityEngine;

namespace MARDEK.Save
{
	public class AddressableMonoBehaviour : MonoBehaviour, IAddressableGuid
	{
		[SerializeField, Newtonsoft.Json.JsonIgnore] SaveOptions saveOptions;
		[Serializable]
		class SaveOptions
		{
			public bool loadOnAwake = true;
			public bool autoSave = true;
			public bool saveOnDisable = true;
		}

		[SerializeField, HideInInspector, Newtonsoft.Json.JsonIgnore]
		private byte[] serializedGuid;
		Guid guid
		{
			get
			{
				if (serializedGuid == null || serializedGuid.Length != 16)
					return Guid.Empty;
				return new Guid(serializedGuid);
			}
			set { serializedGuid = value.ToByteArray(); }
		}

		public Guid GetGuid() { return guid; }
		private void OnValidate()
		{
			ValidateGuid();
		}

		protected virtual void ValidateGuid()
		{
			if (guid == Guid.Empty)
			{
				guid = Guid.NewGuid();
				Debug.Log($"new guid assigned to {this.name}", this);
			}
		}

		protected virtual void Awake()
		{
			if (saveOptions.loadOnAwake)
				Load();
		}

		private void OnEnable()
		{
			if (saveOptions.autoSave)
				SaveSystem.OnBeforeSave += Save;
		}
		private void OnDisable()
		{
			if (saveOptions.autoSave)
				SaveSystem.OnBeforeSave -= Save;
			if (saveOptions.saveOnDisable)
				Save();
		}

		public virtual void Save()
		{
			SaveSystem.SaveObject(this);
		}
		public virtual void Load()
		{
			SaveSystem.LoadObject(this, null);
		}

		public virtual void Load(SaveState saveState)
		{
			SaveSystem.LoadObject(this, saveState);
		}

		[ContextMenu("Save")]
		void SaveWrapper()
		{
			Save();
		}
		[ContextMenu("Load")]
		void LoadWrapper()
		{
			Load();
		}
	}
}