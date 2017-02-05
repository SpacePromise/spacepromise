using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Assets.Engine.Unity;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public static class ExposeProperties
    {
        public static void Expose(PropertyField[] properties)
        {
            var emptyOptions = new GUILayoutOption[0];

            EditorGUILayout.BeginVertical(emptyOptions);

            var lastRow = -1;
            foreach (var field in properties)
            {
                if (lastRow != field.Row && lastRow == 0 && field.Row == 0)
                {
                    if (lastRow != -1)
                        EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(emptyOptions);
                }

                switch (field.Type)
                {
                    case SerializedPropertyType.Integer:
                        if (field.HasSetter)
                            field.SetValue(EditorGUILayout.IntField(field.Name, (int) field.GetValue(), emptyOptions));
                        else EditorGUILayout.LabelField(field.Name, field.GetValue().ToString(), emptyOptions);
                        break;

                    case SerializedPropertyType.Float:
                        if (field.HasSetter)
                            field.SetValue(EditorGUILayout.FloatField(field.Name, (float) field.GetValue(), emptyOptions));
                        else EditorGUILayout.LabelField(field.Name, ((float)field.GetValue()).ToString(CultureInfo.InvariantCulture), emptyOptions);
                        break;

                    case SerializedPropertyType.Boolean:
                        field.SetValue(field.IsButton
                            ? GUILayout.Button(field.Name, emptyOptions)
                            : EditorGUILayout.Toggle(field.Name, (bool) field.GetValue(), emptyOptions));
                        break;

                    case SerializedPropertyType.String:
                        field.SetValue(EditorGUILayout.TextField(field.Name, (String) field.GetValue(), emptyOptions));
                        break;

                    case SerializedPropertyType.Vector2:
                        field.SetValue(EditorGUILayout.Vector2Field(field.Name, (Vector2) field.GetValue(), emptyOptions));
                        break;

                    case SerializedPropertyType.Vector3:
                        field.SetValue(EditorGUILayout.Vector3Field(field.Name, (Vector3) field.GetValue(), emptyOptions));
                        break;

                    case SerializedPropertyType.Enum:
                        if (field.HasSetter)
                            field.SetValue(EditorGUILayout.EnumPopup(field.Name, (Enum) field.GetValue(), emptyOptions));
                        else EditorGUILayout.LabelField(field.Name, ((Enum)field.GetValue()).ToString(), emptyOptions);
                        break;

                    case SerializedPropertyType.ObjectReference:
                        field.SetValue(EditorGUILayout.ObjectField(field.Name, (UnityEngine.Object) field.GetValue(),
                            field.GetPropertyType(), true, emptyOptions));
                        break;
                }

                lastRow = field.Row;
            }

            EditorGUILayout.EndVertical();

        }

        public static PropertyField[] GetProperties(System.Object obj)
        {

            var fields = new List<PropertyField>();
            var infos = obj
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var info in infos)
            {
                var attributes = info.GetCustomAttributes(true);

                var exposed = attributes.FirstOrDefault(o => o is ExposePropertyAttribute) as ExposePropertyAttribute;
                if (exposed == null)
                    continue;

                SerializedPropertyType type;
                if (!PropertyField.GetPropertyType(info, out type))
                    continue;

                fields.Add(new PropertyField(obj, info, type, exposed));
            }

            return fields.ToArray();
        }
    }
}