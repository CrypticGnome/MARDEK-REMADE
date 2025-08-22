using UnityEngine;
using System.Collections.Generic;
using System;
namespace MARDEK
{

    public class DesignNotes : MonoBehaviour
    {
#if UNITY_EDITOR
          [SerializeField] List<Note> notes;
          [Serializable]
          class Note
          {
               public string Header;
               [SerializeField, TextAreaAttribute] public string Notes;
          }

#endif

     }
}
