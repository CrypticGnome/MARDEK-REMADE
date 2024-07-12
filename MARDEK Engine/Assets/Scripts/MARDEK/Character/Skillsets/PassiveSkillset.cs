using MARDEK.Skill;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MARDEK/Skill/Passive Skillsets")]
public class PassiveSkillset : ScriptableObject
{
     [field: SerializeField] public string Description { get; private set; }
     [field: SerializeField] public Sprite Sprite { get; private set; }
     [field: SerializeField] public List<PassiveSkill> Skills { get; private set; }
}