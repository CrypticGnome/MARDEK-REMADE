

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
namespace MARDEK.Event
{
     [CustomPropertyDrawer(typeof(CommandBranch))]
     public class CommandBranchPropertyDrawer : PropertyDrawer
     {
          #region Overrides
          public override VisualElement CreatePropertyGUI(SerializedProperty property)
          {
               var visualElement = new VisualElement();

               //var propertyField = new PropertyField();
               // propertyField.BindProperty(property);

               var lockPlayerActions = new PropertyField(property.FindPropertyRelative("lockPlayerActions"));
               var onTrue = new PropertyField(property.FindPropertyRelative($"OnTrue"));
               var onFalse = new PropertyField(property.FindPropertyRelative("OnFalse"));

               return visualElement;
          }
          #endregion Overrides
     }
}
#endif