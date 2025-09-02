using UnityEngine;

namespace MARDEK
{
    public class DebugGameObjectEnabled : MonoBehaviour
    {
          private void OnEnable()
          {
               Debug.Log("DEBUGGER ENABLED");
          }
     }
}
