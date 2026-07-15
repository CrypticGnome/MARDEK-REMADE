using System.Collections.Generic;
using MARDEK.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Guid = System.Guid;

namespace MARDEK.Save
{
	[System.Serializable]
	public class SaveState
	{
		public Dictionary<Guid, JObject> addressableState = new Dictionary<Guid, JObject>();

		public void SaveObject(IAddressableGuid addressable, JsonSerializer serializer)
		{
			Guid guid = addressable.GetGuid();
			addressableState[guid] = JObject.FromObject(addressable, serializer);
		}

		public bool LoadObject(IAddressableGuid addressable, JsonSerializer serializer)
		{
			Guid guid = addressable.GetGuid();
			if (addressableState.TryGetValue(guid, out JObject data) == false || data == null)
				return false;

			// Addressable found, populate the live object from its saved data
			using (JsonReader reader = data.CreateReader())
				serializer.Populate(reader, addressable);
			return true;
		}
	}
}
