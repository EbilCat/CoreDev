using System;
using System.Collections.Generic;
using System.Reflection;

public static class ReflectionUtils
{

    /// <summary>
    /// Traverses the inheritance tree for a given type and retrieves all PropertyInfo
    /// </summary>
    /// <param name="type">The Type whose PropertyInfos we want to retrieve</param>
    /// <param name="bindingFlags">BindingFlags</param>
    /// <returns>List containing all PropertyInfo relating to type</returns>
    public static List<PropertyInfo> GetAllPropertyInfo(this Type type, BindingFlags bindingFlags, List<PropertyInfo> allPropertyInfo = null)
    {
        if (allPropertyInfo == null) { allPropertyInfo = new List<PropertyInfo>(); }

        if (type.BaseType != null)
        {
            type.BaseType.GetAllPropertyInfo(bindingFlags, allPropertyInfo);
        }

        PropertyInfo[] properties = type.GetProperties(bindingFlags);

        for (int i = 0; i < properties.Length; i++)
        {
            PropertyInfo propertyInfo = properties[i];
            string propertyName = propertyInfo.Name;

            if (allPropertyInfo.Find((x) => { return (x.Name == propertyName); }) == null)
            {
                allPropertyInfo.Add(propertyInfo);
            }
        }

        return allPropertyInfo;
    }

    /// <summary>
    /// Traverses the inheritance tree for a given type and retrieves all FieldInfo
    /// </summary>
    /// <param name="type">The Type whose FieldInfos we want to retrieve</param>
    /// <param name="bindingFlags">BindingFlags</param>
    /// <returns>List containing all FieldInfo relating to type</returns>
    public static List<FieldInfo> GetAllFieldInfo(this Type type, BindingFlags bindingFlags, List<FieldInfo> allFieldInfo = null)
    {
        if (allFieldInfo == null) { allFieldInfo = new List<FieldInfo>(); }

        if (type.BaseType != null)
        {
            GetAllFieldInfo(type.BaseType, bindingFlags, allFieldInfo);
        }
        
        FieldInfo[] fields = type.GetFields(bindingFlags);

        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo fieldInfo = fields[i];
            string fieldName = fieldInfo.Name;
            if (allFieldInfo.Find((x) => { return (x.Name == fieldName); }) == null)
            {
                allFieldInfo.Add(fieldInfo);
            }
        }

        return allFieldInfo;
    }



    public static List<FieldInfo> GetNameAttributedFieldInfo(this Type classType, string name, Type fieldType, List<FieldInfo> namedAttributedFieldInfo = null)
    {
        if (namedAttributedFieldInfo == null) { namedAttributedFieldInfo = new List<FieldInfo>(); }

        FieldInfo[] fieldInfos = classType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo info in fieldInfos)
        {
            NamedVarAttribute attribute = info.GetCustomAttribute<NamedVarAttribute>();
            if (attribute != null && info.FieldType.IsSubclassOf(fieldType))
            {
                namedAttributedFieldInfo.Add(info);
            }
        }
        return namedAttributedFieldInfo;
    }


    public static T GetValueOfNamedAttribute<T>(this object obj, string name)
    {
        FieldInfo[] fieldInfos = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

        foreach (FieldInfo info in fieldInfos)
        {
            NamedVarAttribute attribute = info.GetCustomAttribute<NamedVarAttribute>();
            if (attribute != null && attribute.name == name && info.FieldType.IsSubclassOf(typeof(T)))
            {
                T retrievedValue = (T)info.GetValue(obj);
                return retrievedValue;
            }
        }

        return default(T);
    }
}


[System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property)]
public class NamedVarAttribute : System.Attribute
{
    public string name { get; private set; }

    public NamedVarAttribute(string name)
    {
        this.name = name;
    }
}