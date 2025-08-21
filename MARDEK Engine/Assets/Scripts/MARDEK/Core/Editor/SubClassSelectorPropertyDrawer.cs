#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

//https://discussions.unity.com/t/abstract-skilllist-that-can-be-edited-in-inspector/948612/15
[CustomPropertyDrawer(typeof(SubclassSelectorAttribute))]
public sealed class SubclassSelectorPropertyDrawer : PropertyDrawer
{
     #region Overrides

     public override VisualElement CreatePropertyGUI(SerializedProperty property)
     {
          var visualElement = new VisualElement();

          var propertyField = new PropertyField();
          propertyField.BindProperty(property);
          propertyField.label = " ";

          if (property.propertyType != SerializedPropertyType.ManagedReference)
          {
               visualElement.Add(propertyField);
               return visualElement;
          }

          var types = GetTypes(fieldInfo, property);

          Box box = new Box();
          visualElement.Add(box);
          #region AssignBox
          box.style.marginTop = 2;
          box.style.marginBottom = 2;
          box.style.paddingTop = 4;
          box.style.paddingBottom = 4;
          box.style.paddingLeft = 4;
          box.style.paddingRight = 4;
          box.style.borderLeftWidth = 1;
          box.style.borderRightWidth = 1;
          box.style.borderTopWidth = 1;
          box.style.borderBottomWidth = 1;
          #endregion AssignBox

          var dropdownField = new TypePopupField(property, types);
          box.Add(dropdownField);

          box.Add(propertyField);

          
          return visualElement;
     }

     #endregion

     #region Internal Methods

     private static bool IsCollection(Type fieldType)
     {
          if (fieldType.IsArray == true)
          {
               return true;
          }

          if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
          {
               return true;
          }

          return false;
     }

     private static List<Type> GetTypes(FieldInfo fieldInfo, SerializedProperty property)
     {
          var value = property.managedReferenceValue;
          Type currentType = value?.GetType();

          Type fieldType = fieldInfo.FieldType;
          Type baseType;

          bool isCollection = IsCollection(fieldType);

          var types = new List<Type>()
          {
               currentType,
               null
          };

          if (fieldType.IsAbstract == false && isCollection == false)
          {
               types.Add(fieldType);
          }

          if (isCollection == true)
          {
               baseType = fieldType.GetGenericArguments()[0];
               if (baseType.IsAbstract == false)
               {
                    types.Add(baseType);
               }
          }
          else
          {
               baseType = fieldType;
          }

          var derivedTypes = TypeCache.GetTypesDerivedFrom(baseType);
          foreach (var derivedType in derivedTypes)
          {
               types.Add(derivedType);
          }

          return types;
     }

     #endregion

     #region Internal Types

     public sealed class TypePopupField : PopupField<Type>
     {
          #region Internal Members

          private readonly SerializedProperty _property;

          #endregion

          public TypePopupField(SerializedProperty property, List<Type> types) : base(property.displayName, types, 0, GetTypeName, GetTypeName)
          {
               _property = property;
               this.RegisterValueChangedCallback(OnValueSelected);
          }

          #region Internal Methods

          private void OnValueSelected(ChangeEvent<Type> changeEvent)
          {
               Type selectedType = changeEvent.newValue;
               if (selectedType == null)
               {
                    _property.managedReferenceValue = null;
                    _property.serializedObject.ApplyModifiedProperties();
               }
               else
               {
                    var constructor = selectedType.GetConstructor(Type.EmptyTypes);
                    if (constructor != null)
                    {
                         var value = constructor.Invoke(null);
                         _property.managedReferenceValue = value;
                         _property.serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                         Debug.LogWarning($"Selected Type {selectedType.Name} does not have a parameterless constructor. Cannot assign instance of type.");
                    }
               }
          }

          private static string GetTypeName(Type type)
          {
               if (type == null)
               {
                    return "Null";
               }
               else
               {
                    return ObjectNames.NicifyVariableName(type.Name);
               }
          }

          #endregion
     }

     #endregion
}
#endif