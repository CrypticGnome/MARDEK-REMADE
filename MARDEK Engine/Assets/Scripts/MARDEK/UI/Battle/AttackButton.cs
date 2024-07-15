using MARDEK.Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MARDEK.UI
{
    public class AttackButton : MonoBehaviour
    {
        //[SerializeField] Image sprite;
        [SerializeField] ActionSkill attack;

        public void SelectAction()
        {
            Battle.BattleManager.PerformAction(attack.Apply);
        }
    }
}