using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MARDEK.Core
{
	// Serializes addressable assets as { "refGuid": "<guid>" } and resolves them
	// back through the AddressableDatabase instead of creating new instances
	public class GuidReferenceConverter : JsonConverter
	{
		const string refGuidFieldName = "refGuid";

		public override bool CanConvert(Type objectType)
		{
			return typeof(AddressableScriptableObject).IsAssignableFrom(objectType);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var addressable = (IAddressableGuid)value;
			writer.WriteStartObject();
			writer.WritePropertyName(refGuidFieldName);
			writer.WriteValue(addressable.GetGuid().ToString());
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
				return null;

			//shouldn't create an instance of an addressable, get reference from database instead
			var data = JObject.Load(reader);
			var guid = data.Value<string>(refGuidFieldName);
			if (string.IsNullOrEmpty(guid))
				return null;
			return AddressableDatabase.GetAddressableByGuid(guid);
		}
	}
}
