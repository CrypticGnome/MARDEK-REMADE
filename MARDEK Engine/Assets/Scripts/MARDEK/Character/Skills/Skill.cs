using MARDEK.Core;
using UnityEngine;

namespace MARDEK.Skill
{
	public class Skill : AddressableScriptableObject
	{
		[field: SerializeField] public int PointsRequiredToMaster { get; private set; }
		[field: SerializeField] public string DisplayName { get; private set; }
		[field: SerializeField] public string Description { get; private set; }
	}
}