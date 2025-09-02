using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace MARDEK.Movement
{
     [CustomPropertyDrawer(typeof(Moves))]
     public class MovesDrawer : PropertyDrawer
     {
          public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
          {
               // Draw prefix label ("Moves") correctly
               position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

               // Find the properties
               SerializedProperty directionProp = property.FindPropertyRelative("Direction");
               SerializedProperty countProp = property.FindPropertyRelative("Count");

               // Split the available space
               float spacing = 5f;
               float thirdWidth = (position.width - spacing) / 3f;

               Rect directionRect = new Rect(position.x, position.y, 2 * thirdWidth, position.height);
               Rect countRect = new Rect(position.x + 2* thirdWidth + spacing, position.y, thirdWidth, position.height);

               // Draw fields
               EditorGUI.PropertyField(directionRect, directionProp, GUIContent.none);
               EditorGUI.PropertyField(countRect, countProp, GUIContent.none);
          }
     }
}
#endif