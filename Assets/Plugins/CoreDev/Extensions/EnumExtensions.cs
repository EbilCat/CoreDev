using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace CoreDev.Extensions
{
    public static class EnumExtensions
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static T ToEnum<T>(this string enumString)
        {
            T enumValue = (T)Enum.Parse(typeof(T), enumString, true);
            return enumValue;
        }

        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);

            //Find a Description Attribute for a potential friendly name for the enum
            FieldInfo field = type.GetField(name);
            if (field != null)
            {
                DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attr != null) { return attr.Description; }
            }

            // No description attribute, return the ToString of the enum
            return value.ToString();
        }

        public static T DescriptionToEnum<T>(this string enumString) where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            if (enumType.IsEnum)
            {
                if (!string.IsNullOrEmpty(enumString))
                {
                    foreach (T val in Enum.GetValues(enumType))
                    {
                        Enum value = (Enum)(object)val;
                        if (value.GetDescription().Trim().ToLower().Equals(enumString.Trim().ToLower()))
                        {
                            return (T)(object)value;
                        }
                    }
                }
                throw new ArgumentException(string.Format("EnumExtensions.DescriptionToEnum: {0} is not a Description of the {1} enumeration.", enumString, enumType));
            }
            else
            {
                throw new ArgumentException(string.Format("EnumExtensions.DescriptionToEnum: {0} is not an Enum type.", enumType));
            }
        }
    }
}
