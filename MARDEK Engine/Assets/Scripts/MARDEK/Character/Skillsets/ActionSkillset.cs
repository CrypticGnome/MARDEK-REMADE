using MARDEK.Skill;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MARDEK/Skill/Action Skillsets")]
public class ActionSkillset : ScriptableObject
{
     [field: SerializeField] public string Description { get; private set; }
     [field: SerializeField] public Sprite Sprite { get; private set; }
     [field: SerializeField] public List<ActionSkill> Skills { get; private set; }

}