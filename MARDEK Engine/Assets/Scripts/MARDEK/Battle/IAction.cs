using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MARDEK.Battle
{
     using MARDEK.CharacterSystem;
     public interface IAction
     {
          public void Apply(Character user, Character target);
     }
}