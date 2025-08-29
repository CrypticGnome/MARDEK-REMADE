using UnityEngine;

public static class FloatExtensions
{
     public static float Clamp0Positive(this float a) => a < 0 ? 0 : a;
}
