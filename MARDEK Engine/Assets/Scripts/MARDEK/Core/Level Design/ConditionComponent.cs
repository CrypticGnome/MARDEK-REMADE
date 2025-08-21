using MARDEK.Core.LevelDesign;
using UnityEngine;

public class ConditionComponent : MonoBehaviour
{
     [SerializeReference, SubclassSelector] Condition Condition;
}
