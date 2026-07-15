using System;
using System.Collections.Generic;
using System.Reflection;
using MARDEK.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace MARDEK.Save
{
	// Serializes objects the way Unity does: public fields plus [SerializeField]
	// private fields, no properties. UnityEngine.Object references are skipped
	// entirely unless they are addressable assets (those are saved by GUID
	// reference through the GuidReferenceConverter)
	public class SaveContractResolver : DefaultContractResolver
	{
		protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			var properties = new List<JsonProperty>();
			var propertyNames = new HashSet<string>();
			const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

			// walk the hierarchy manually so private [SerializeField] fields of base classes are included
			for (Type currentType = type; currentType != null && currentType != typeof(object); currentType = currentType.BaseType)
			{
				foreach (FieldInfo field in currentType.GetFields(flags))
				{
					if (ShouldSerialize(field) == false)
						continue;
					if (propertyNames.Add(field.Name) == false)
						continue; // field shadowed by a derived class, keep the derived one

					JsonProperty property = CreateProperty(field, memberSerialization);
					property.Readable = true;
					property.Writable = true;
					properties.Add(property);
				}
			}
			return properties;
		}

		static bool ShouldSerialize(FieldInfo field)
		{
			if (field.IsInitOnly || field.IsLiteral)
				return false;
			if (field.IsDefined(typeof(NonSerializedAttribute)) || field.IsDefined(typeof(JsonIgnoreAttribute)))
				return false;
			if (field.IsPublic == false && field.IsDefined(typeof(SerializeField)) == false)
				return false;

			// scene/asset references can't be meaningfully persisted, except addressables (saved by GUID)
			if (typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType)
				&& typeof(AddressableScriptableObject).IsAssignableFrom(field.FieldType) == false)
				return false;

			return true;
		}
	}
}
