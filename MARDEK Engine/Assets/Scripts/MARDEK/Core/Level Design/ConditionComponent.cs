using MARDEK.Core.LevelDesign;
using UnityEngine;

public class ConditionComponent : MonoBehaviour
{
     [SerializeReference, SubclassSelector] public Condition Condition;
}
