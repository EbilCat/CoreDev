using System;
using System.Collections;
using System.Reflection;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;


namespace CoreDev.DataObjectInspector
{
    public class ObservableVarInfoDO : IDataObject
    {
        private string[] newValueArray = new string[1];
        private object[] eventHandlerHolder = new object[1];

        private FieldInfo fieldInfo;
        private PropertyInfo valuePropertyInfo;
        private MethodInfo setValueFromStringMethodInfo;
        private EventInfo valueChangeBlockedEventInfo;
        private EventInfo valueChangedEventInfo;
        private EventInfo moderatorsChangedEventInfo;
        private EventInfo callbacksChangedEventInfo;

        public Type EnclosedValueType { get; private set; }
        public bool IsCollection { get; private set; }

        public string Name => fieldInfo.Name;
        public Type FieldType => fieldInfo.FieldType;
        public PropertyInfo ValuePropertyInfo => valuePropertyInfo;
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
            this.valueChangeBlockedEventInfo = this.FieldType.GetEvent("ValueChangeBlocked", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            this.valueChangedEventInfo = this.FieldType.GetEvent("ValueChanged", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            this.moderatorsChangedEventInfo = this.FieldType.GetEvent("ModeratorsChanged", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            this.callbacksChangedEventInfo = this.FieldType.GetEvent("CallbacksChanged", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            this.EnclosedValueType = this.valuePropertyInfo?.PropertyType;
            this.IsCollection = typeof(ICollection).IsAssignableFrom(this.EnclosedValueType);

            this.isExpandedView = new OBool(false, this);
            this.isBookedMarked = new OBool(false, this);
            this.orderIndex = new OInt(orderIndex, this);
        }


        public event Action<IDataObject> disposing;
        public void Dispose()
        {
            disposing?.Invoke(this);
        }


//*====================
//* PUBLIC
//*====================
        public IObservableVar GetObservableVarInstance(IDataObject dataObjectInstance)
        {
            IObservableVar oVar = fieldInfo.GetValue(dataObjectInstance) as IObservableVar;

            if (oVar == null)
            {
                Debug.LogError($"ObservableVar {fieldInfo.Name} in {dataObjectInstance.GetType().Name} is NULL. It has likely not been initialized.");
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

        public void RegisterForValueChangeBlocks(IObservableVar observableVarInstance, Action<string> callback)
        {
            eventHandlerHolder[0] = callback;
            valueChangeBlockedEventInfo.GetRemoveMethod(true).Invoke(observableVarInstance, eventHandlerHolder);
            valueChangeBlockedEventInfo.GetAddMethod(true).Invoke(observableVarInstance, eventHandlerHolder);
        }

        public void UnregisterFromValueChangeBlocks(IObservableVar observableVarInstance, Action<string> callback)
        {
            eventHandlerHolder[0] = callback;
            valueChangeBlockedEventInfo.GetRemoveMethod(true).Invoke(observableVarInstance, eventHandlerHolder);
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

        public void RegisterForCallbackChanges(IObservableVar observableVarInstance, Action callback)
        {
            eventHandlerHolder[0] = callback;
            callbacksChangedEventInfo.GetRemoveMethod(true).Invoke(observableVarInstance, eventHandlerHolder);
            callbacksChangedEventInfo.GetAddMethod(true).Invoke(observableVarInstance, eventHandlerHolder);
        }
        
        public void UnregisterFromCallbackChanges(IObservableVar observableVarInstance, Action callback)
        {
            eventHandlerHolder[0] = callback;
            callbacksChangedEventInfo.GetRemoveMethod(true).Invoke(observableVarInstance, eventHandlerHolder);
        }
    }
}