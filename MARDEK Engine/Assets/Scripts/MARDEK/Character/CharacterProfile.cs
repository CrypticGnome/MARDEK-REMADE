﻿using UnityEngine;
using MARDEK.Core;
using MARDEK.Animation;
using MARDEK.Skill;
using MARDEK.Stats;

namespace MARDEK.CharacterSystem
{
    [CreateAssetMenu(menuName ="MARDEK/Character/CharacterProfile")]
    public class CharacterProfile : AddressableScriptableObject
    {
          [field: SerializeField] public string displayName { get; private set; }
          [field: SerializeField] public string displayClass { get; private set; }
          [field: SerializeField] public Element element { get; private set; }
          [field: SerializeField] public CharacterPortrait portrait { get; private set; }
          [field: SerializeField] public SpriteAnimationClipList WalkSprites { get; private set; }
          [field: SerializeField] public GameObject BattleModelPrefab { get; private set; }
          [field: SerializeField] public ActionSkillset ActionSkillset { get; private set; }
          [field: SerializeField] public ReactionSkillset ReactionSkillset { get; private set; }
          [field: SerializeField] public PassiveSkillset PassiveSkillset { get; private set; }
          [field: SerializeField] public StatsClass Stats { get; private set; } = new StatsClass();  // Stats namespace needs to be removed
    }
}