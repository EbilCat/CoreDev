﻿using System;
using System.Collections;
using System.Reflection;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;

public class ObservableVarInfoDO : IDataObject
{
    private string[] newValueArray = new string[1];
    private object[] eventHandlerHolder = new object[1];

    private FieldInfo fieldInfo;
    private PropertyInfo valuePropertyInfo;
    private MethodInfo setValueFromStringMethodInfo;
    private EventInfo valueChangedEventInfo;
    private EventInfo moderatorsChangedEventInfo;

    public Type EnclosedValueType { get; private set; }
    public bool IsCollection { get; private set; }

    public string Name { get { return fieldInfo.Name; } }
    public Type FieldType { get { return fieldInfo.FieldType; } }

    public OBool isExpandedView;
    public OBool isBookedMarked;
    public OInt orderIndex;


//*====================
//* CONSTRUCTOR
//*====================
    public ObservableVarInfoDO(FieldInfo fieldInfo, int orderIndex)
    {
        this.fieldInfo = fieldInfo;
        this.valuePropertyInfo = this.FieldType.GetProperty("Value");
        this.setValueFromStringMethodInfo = this.FieldType.GetMethod("SetValueFromString");
        this.valueChangedEventInfo = this.FieldType.GetEvent("ValueChanged", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        this.moderatorsChangedEventInfo = this.FieldType.GetEvent("ModeratorsChanged", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

        this.EnclosedValueType = this.valuePropertyInfo.PropertyType;
        this.IsCollection = typeof(ICollection).IsAssignableFrom(this.EnclosedValueType);

        this.isExpandedView = new OBool(false, this);
        this.isBookedMarked = new OBool(false, this);
        this.orderIndex = new OInt(orderIndex, this);
    }

    public void Dispose()
    {
    }


//*====================
//* PUBLIC
//*====================
    public IObservableVar GetObservableVarInstance(IDataObject dataObjectInstance)
    {
        IObservableVar oVar = fieldInfo.GetValue(dataObjectInstance) as IObservableVar;

        if (oVar == null)
        {
            Debug.LogError($"ObservableVar {fieldInfo.Name} in {fieldInfo.DeclaringType} is NULL. It has likely not been initialized.");
        }

        return oVar;
    }

    public object GetValue(IObservableVar observableVarInstance)
    {
        object currentValue = valuePropertyInfo.GetValue(observableVarInstance);
        return currentValue;
    }

    public void SetValue(IObservableVar observableVarInstance, string newValue)
    {
        newValueArray[0] = newValue;
        setValueFromStringMethodInfo?.Invoke(observableVarInstance, newValueArray);
    }

    public void RegisterForValueChanges(IObservableVar observableVarInstance, Action callback)
    {
        eventHandlerHolder[0] = callback;
        valueChangedEventInfo.GetRemoveMethod(true).Invoke(observableVarInstance, eventHandlerHolder);
        valueChangedEventInfo.GetAddMethod(true).Invoke(observableVarInstance, eventHandlerHolder);
    }

    public void UnregisterFromValueChanges(IObservableVar observableVarInstance, Action callback)
    {
        eventHandlerHolder[0] = callback;
        valueChangedEventInfo.GetRemoveMethod(true).Invoke(observableVarInstance, eventHandlerHolder);
    }

    public void RegisterForModeratorsChanges(IObservableVar observableVarInstance, Action callback)
    {
        if (moderatorsChangedEventInfo != null)
        {
            eventHandlerHolder[0] = callback;
            moderatorsChangedEventInfo.GetRemoveMethod(true).Invoke(observableVarInstance, eventHandlerHolder);
            moderatorsChangedEventInfo.GetAddMethod(true).Invoke(observableVarInstance, eventHandlerHolder);
        }
    }

    public void UnregisterFromModeratorsChanges(IObservableVar observableVarInstance, Action callback)
    {
        if (moderatorsChangedEventInfo != null)
        {
            eventHandlerHolder[0] = callback;
            moderatorsChangedEventInfo.GetRemoveMethod(true).Invoke(observableVarInstance, eventHandlerHolder);
        }
    }
}