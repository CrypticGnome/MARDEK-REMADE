using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.Battle
{
    using Core;
    using CharacterSystem;
    using MARDEK.Stats;

    public class BattleCharacter : IActor
    {
        public Character Character { get; private set; }
        BattleModel battleModel = null;
        public string Name { get { return Character.Profile.displayName; } }
        public StatsClass Stats { get { return Character.Profile.Stats; } }
        public BattleCharacter(Character character_, Vector3 position)
        {
            Character = character_;
            battleModel = Object.Instantiate(Character.Profile.BattleModelPrefab).GetComponent<BattleModel>();
            battleModel.SetBattlePosition(position);
        }
    }
}