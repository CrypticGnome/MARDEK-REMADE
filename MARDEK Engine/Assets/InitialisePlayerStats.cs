using MARDEK.Progress;
using UnityEngine;

namespace MARDEK.Temp
{
    public class InitialisePlayerStats : MonoBehaviour
    {
          [RuntimeInitializeOnLoadMethod]
          public static void OnStartUp()
          {
               PartySO[] partySO = Resources.LoadAll<PartySO>("");
               if (partySO.Length == 0) 
               {
                    Debug.LogError("No party object found");
                    return;
               }
               if (partySO.Length > 1)
               {
                    Debug.LogError("Only one party object can exist");
                    return;
               }
               var party = partySO[0];
               foreach (var character in party)
               {
                    character.CurrentHP = character.MaxHP;
                    character.CurrentMP = character.MaxMP;
               }
          }
     }
    
}
