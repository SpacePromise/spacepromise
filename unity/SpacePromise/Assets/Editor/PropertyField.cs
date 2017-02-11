using System;
using System.Reflection;
using Assets.Engine.Unity;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public class PropertyField
    {
        private readonly System.Object instance;
        private readonly PropertyInfo info;
        private readonly MethodInfo getter;
        private readonly MethodInfo setter;
        private readonly string instanceTypeName;


        public PropertyField(System.Object instance, PropertyInfo info, SerializedPropertyType type)
            : this(instance, info, type, null)
        {
        }

        public PropertyField(System.Object instance, PropertyInfo info, SerializedPropertyType type, ExposePropertyAttribute exposePropertyAttribute)
        {
            this.instance = instance;
            this.info = info;
            Type = type;

            this.instanceTypeName = instance.GetType().Name;

            getter = this.info.GetGetMethod();
            setter = this.info.GetSetMethod();

            if (exposePropertyAttribute == null)
                return;

            this.Row = exposePropertyAttribute.Row;

            var buttonProperty = exposePropertyAttribute as ExposePropertyButtonAttribute;
            if (buttonProperty != null)
                this.IsButton = true;
        }

        public System.Object GetValue()
        {
            return getter.Invoke(instance, null);
        }

        public void SetValue(System.Object value)
        {
            setter.Invoke(instance, new[] { value });
        }

        public Type GetPropertyType()
        {
            return info.PropertyType;
        }

        public static bool GetPropertyType(PropertyInfo info, out SerializedPropertyType propertyType)
        {
            var type = info.PropertyType;

            if (type == typeof(int))
            {
                propertyType = SerializedPropertyType.Integer;
                return true;
            }

            if (type == typeof(float))
            {
                propertyType = SerializedPropertyType.Float;
                return true;
            }

            if (type == typeof(bool))
            {
                propertyType = SerializedPropertyType.Boolean;
                return true;
            }

            if (type == typeof(string))
            {
                propertyType = SerializedPropertyType.String;
                return true;
            }

            if (type == typeof(Vector2))
            {
                propertyType = SerializedPropertyType.Vector2;
                return true;
            }

            if (type == typeof(Vector3))
            {
                propertyType = SerializedPropertyType.Vector3;
                return true;
            }

            if (type.IsEnum)
            {
                propertyType = SerializedPropertyType.Enum;
                return true;
            }
        
            propertyType = SerializedPropertyType.ObjectReference;
            return true;
        }


        public string InstanceTypeName
        {
            get { return this.instanceTypeName; }
        }

        public bool HasGetter
        {
            get { return getter != null; }
        }

        public bool HasSetter
        {
            get { return setter != null; }
        }

        public SerializedPropertyType Type { get; private set; }

        public string Name
        {
            get { return ObjectNames.NicifyVariableName(info.Name); }
        }

        public int Row { get; private set; }

        public bool IsButton { get; private set; }
    }
}