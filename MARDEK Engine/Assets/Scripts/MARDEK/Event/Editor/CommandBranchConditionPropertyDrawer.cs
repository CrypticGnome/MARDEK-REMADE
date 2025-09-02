using UnityEditor;
using UnityEngine;
using static MARDEK.Event.CommandBranch;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(CommandBranchCondition))]
public class CommandBranchConditionPropertyDrawer : PropertyDrawer
{
     private readonly string[] options = { "Local Switch Bool", "Condition Component" };

     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
     {
          EditorGUI.BeginProperty(position, label, property);

          // Get properties
          var usingSwitchBoolProp = property.FindPropertyRelative("UsingSwitchBool");
          var localSwitchBoolProp = property.FindPropertyRelative("LocalSwitchBool");
          var conditionComponentProp = property.FindPropertyRelative("ConditionComponent");

          // Layout: dropdown on left, field on right
          Rect dropdownRect = new Rect(position.x, position.y, 150, EditorGUIUtility.singleLineHeight);
          Rect fieldRect = new Rect(position.x + dropdownRect.width + 4, position.y,
                                    position.width - dropdownRect.width - 4, EditorGUIUtility.singleLineHeight);

          // Draw dropdown
          int index = usingSwitchBoolProp.boolValue ? 0 : 1;
          index = EditorGUI.Popup(dropdownRect, index, options);

          // Update bool based on selection
          usingSwitchBoolProp.boolValue = (index == 0);

          // Draw the chosen field
          if (usingSwitchBoolProp.boolValue)
               EditorGUI.PropertyField(fieldRect, localSwitchBoolProp, GUIContent.none);
          else
               EditorGUI.PropertyField(fieldRect, conditionComponentProp, GUIContent.none);

          EditorGUI.EndProperty();
     }

     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
     {
          return EditorGUIUtility.singleLineHeight;
     }
}
#endif