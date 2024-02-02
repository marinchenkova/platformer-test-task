using System;
using System.Linq;
using Entities.Core;
using Entities.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Entities.Editor.Components {

    [CustomPropertyDrawer(typeof(IEntityComponent), useForChildren: true)]
    public sealed class EntityComponentPropertyDrawer : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            object value = property.managedReferenceValue;
            var type = value?.GetType();
            var typeLabel = new GUIContent(type == null ? "null" : type.Name);

            float popupWidth = position.width - EditorGUIUtility.labelWidth;

            var popupPosition = new Rect(
                position.x + position.width - popupWidth,
                position.y,
                popupWidth,
                EditorGUIUtility.singleLineHeight
            );

            if (EditorGUI.DropdownButton(popupPosition, typeLabel, FocusType.Keyboard)) {
                CreateTypeDropdown(typeof(IEntityComponent), property).Show(popupPosition);
            }

            EditorGUI.PropertyField(position, property, label, includeChildren: true);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if (property.managedReferenceValue == null) return EditorGUIUtility.singleLineHeight;
            return EditorGUI.GetPropertyHeight(property, label, includeChildren: true);
        }

        private static AdvancedDropdown<Type> CreateTypeDropdown(Type baseType, SerializedProperty property) {
            var types = TypeCache
                .GetTypesDerivedFrom(baseType)
                .Where(IsSupportedType)
                .Append(null)
                .OrderBy(t => t != null)
                .ThenBy(t => t?.Name ?? string.Empty);

            return new AdvancedDropdown<Type>(
                "Select type",
                types,
                type => type == null ? "null" : type.Name,
                type => CreateSerializedReferenceInstance(type, property.Copy())
            );
        }

        private static void CreateSerializedReferenceInstance(Type type, SerializedProperty property) {
            object value = type == null ? null : Activator.CreateInstance(type);

            property.managedReferenceValue = value;
            property.isExpanded = value != null;

            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }

        private static bool IsSupportedType(Type t) {
            return (t.IsPublic || t.IsNestedPublic) &&
                   !t.IsAbstract &&
                   !t.IsGenericType &&
                   !t.IsValueType &&
                   Attribute.IsDefined(t, typeof(SerializableAttribute));
        }
    }

}
