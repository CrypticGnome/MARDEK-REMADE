using MARDEK.Progress;
using UnityEngine;

namespace MARDEK.Temp
{
    public class InitialisePlayerStats : MonoBehaviour
    {
          [SerializeField] PartySO party;
          static public bool Initialised = false;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
               if (Initialised) return;
               Initialised = true;
               foreach (var character in party)
               {
                    character.CurrentHP = character.MaxHP;
                    character.CurrentMP = character.MaxMP;
               }
          }

    }
}
