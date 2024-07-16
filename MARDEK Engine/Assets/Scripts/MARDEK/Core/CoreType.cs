using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CoreType
{
     [SerializeField] int Int;
     [SerializeField] float Float;
     [SerializeField] bool Bool;


     public enum Type
     {
          Int,
          Float,
          Bool
     }
}
