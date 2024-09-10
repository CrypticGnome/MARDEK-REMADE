using MARDEK.CharacterSystem;
using UnityEngine;
namespace MARDEK.CharacterSystem
{
     [CreateAssetMenu(fileName = "CharacterUnplayable", menuName = "MARDEK/Character/Playable Character")]

     public class CharacterPlayable : Character
     {
          [field: SerializeField] public ReactionSkillset ReactionSkillset { get; private set; }
          [field: SerializeField] public PassiveSkillset PassiveSkillset { get; private set; }
     }
}