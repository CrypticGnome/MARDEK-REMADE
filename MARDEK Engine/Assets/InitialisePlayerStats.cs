using MARDEK.Progress;
using UnityEngine;

namespace MARDEK.Temp
{
    public class InitialisePlayerStats : MonoBehaviour
    {
          [RuntimeInitializeOnLoadMethod]
          public static void OnStartUp()
          {
               PartySO party = Resources.Load<PartySO>("Current Party");
               if (!party) 
               {
                    Debug.LogError("No party object found");
                    return;
               }

               foreach (var character in party)
               {
                    character.CurrentHP = character.MaxHP;
                    character.CurrentMP = character.MaxMP;
               }
          }
     }
    
}
