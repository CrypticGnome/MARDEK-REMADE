using System;
using UnityEngine;

namespace MARDEK.Core
{
	// Saved by GUID reference, not by value - see GuidReferenceConverter,
	// registered in SaveSystem's serializer settings
	public abstract class AddressableScriptableObject : ScriptableObject, IAddressableGuid
	{
		public Guid GetGuid()
		{
			return AddressableDatabase.GetGUID(this);
		}
	}
}
