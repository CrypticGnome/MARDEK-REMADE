using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.Skill
{
    [CreateAssetMenu(menuName = "MARDEK/Skill/Skillset")]
    public class Skillset : ScriptableObject
    {
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public List<Skill> Skills { get; private set; }
    }
}