using MARDEK.Skill;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MARDEK/Skill/Reaction Skillsets")]
public class ReactionSkillset : ScriptableObject
{
     [field: SerializeField] public string Description { get; private set; }
     [field: SerializeField] public Sprite Sprite { get; private set; }
     [field: SerializeField] public List<ReactionSkill> Skills { get; private set; }
}