using UnityEngine;

namespace MARDEK.Battle
{
     public interface IBattleAction
     {
          public Sprite ActionIcon { get; }
          public string DisplayName { get; }
          public abstract bool TryPerformAction(BattleCharacter user, BattleCharacter target);
     }
}