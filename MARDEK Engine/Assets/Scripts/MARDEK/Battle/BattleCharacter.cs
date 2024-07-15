using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.Battle
{
    using Core;
    using CharacterSystem;
    using MARDEK.Stats;

    public class BattleCharacter
    {
          public Character Character { get; private set; }
          BattleModel battleModel = null;
          public string Name { get { return Character.Profile.displayName; } }
          public CoreStats Stats { get { return Character.Profile.Stats; } }
          public BattleCharacter(Character character, Vector3 position)
          {
               Character = Object.Instantiate(character);
               battleModel = Object.Instantiate(Character.Profile.BattleModelPrefab).GetComponent<BattleModel>();
               battleModel.SetBattlePosition(position);
               Character.CurrentHP = character.MaxHP;
               Character.CurrentMP = character.MaxMP;
          }
     }
}