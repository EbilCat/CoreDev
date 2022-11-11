using System;
using System.Collections;
using System.Reflection;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;

namespace CoreDev.DataObjectInspector
{
    public class InspectedObservableVarDO : IDataObject
    {
        public InspectedDataObjectDO inspectedDataObject;
        private IObservableVar observableVarInstance;
        public IObservableVar ObservableVarInstance { get { return observableVarInstance; } }

        private ObservableVarInfoDO observableVarInfoDO;
        public ObservableVarInfoDO ObservableVarInfoDO { get { return observableVarInfoDO; } }

        public OBool isInspected;
        public OBool matchesFilter;

        public OString varName;
        public OBool printToConsole;
        public OBool showCallbacks;
        public OEvent Focus;

        public OString cardText;

        public InspectedObservableVarDO(InspectedDataObjectDO inspectedDataObject, IObservableVar observableVarInstance, ObservableVarInfoDO fieldInfoDO, OBool isInspected)
        {
            this.inspectedDataObject = inspectedDataObject;
            this.observableVarInstance = observableVarInstance;
            this.observableVarInfoDO = fieldInfoDO;
            this.isInspected = isInspected;
            this.matchesFilter = new OBool(true, this);
            this.printToConsole = new OBool(false, this);
            this.showCallbacks = new OBool(false, this);
            this.Focus = new OEvent(this);
            this.cardText = new OString(string.Empty, this);

            this.varName = new OString(fieldInfoDO.Name, this);

            this.isInspected.RegisterForChanges(EvaluateEventSubscription);
            this.printToConsole.RegisterForChanges(EvaluateEventSubscription);
        }


        //*====================
        //* IDataObject
        //*====================
        public event Action<IDataObject> disposing;
        public void Dispose()
        {
            disposing?.Invoke(this);
            this.isInspected.UnregisterFromChanges(EvaluateEventSubscription);
            this.printToConsole.UnregisterFromChanges(EvaluateEventSubscription);

            observableVarInfoDO.UnregisterFromValueChanges(observableVarInstance, RefreshCardText);
            observableVarInfoDO.UnregisterFromValueChanges(observableVarInstance, OnValueChanged);
            observableVarInfoDO.UnregisterFromValueChangeBlocks(observableVarInstance, OnValueChangeBlocked);
            observableVarInfoDO.UnregisterFromModeratorsChanges(observableVarInstance, OnModeratorsChanged);
        }


        //*====================
        //* CALLBACKS
        //*====================
        private void EvaluateEventSubscription(ObservableVar<bool> obj)
        {
            if (this.isInspected.Value || this.printToConsole.Value)
            {
                this.observableVarInfoDO.RegisterForValueChanges(observableVarInstance, RefreshCardText);
                this.observableVarInfoDO.RegisterForModeratorsChanges(observableVarInstance, RefreshCardText);
                this.observableVarInfoDO.isExpandedView.RegisterForChanges(OnIsExpandedViewChanged);
                this.RefreshCardText();
            }
            else
            {
                this.observableVarInfoDO.UnregisterFromValueChanges(observableVarInstance, RefreshCardText);
                this.observableVarInfoDO.UnregisterFromModeratorsChanges(observableVarInstance, RefreshCardText);
                this.observableVarInfoDO?.isExpandedView.UnregisterFromChanges(OnIsExpandedViewChanged);
            }

            if (this.printToConsole.Value)
            {
                this.observableVarInfoDO.RegisterForValueChanges(observableVarInstance, OnValueChanged);
                this.observableVarInfoDO.RegisterForValueChangeBlocks(observableVarInstance, OnValueChangeBlocked);
                this.observableVarInfoDO.RegisterForModeratorsChanges(observableVarInstance, OnModeratorsChanged);
            }
            else
            {
                this.observableVarInfoDO.UnregisterFromValueChanges(observableVarInstance, OnValueChanged);
                this.observableVarInfoDO.UnregisterFromValueChangeBlocks(observableVarInstance, OnValueChangeBlocked);
                this.observableVarInfoDO.UnregisterFromModeratorsChanges(observableVarInstance, OnModeratorsChanged);
            }
        }

        private void OnIsExpandedViewChanged(ObservableVar<bool> obj)
        {
            this.RefreshCardText();
        }

        private void OnValueChanged()
        {
            Debug.LogFormat("[Frame {0}] {1} {2} => {3}", Time.frameCount, this.inspectedDataObject.DataObjectInstance, observableVarInfoDO.Name, this.observableVarInstance);
        }

        private void OnModeratorsChanged()
        {
            Debug.LogFormat("[Frame {0}] Moderator on {1} updated", Time.frameCount, observableVarInfoDO.Name);
        }

        private void OnValueChangeBlocked(string moderatorName)
        {
            Debug.LogFormat("[Frame {0}] Change to {1} blocked by Moderator: {2}", Time.frameCount, observableVarInfoDO.Name, moderatorName);
        }


        //*====================
        //* PRIVATE
        //*====================
        private void RefreshCardText()
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

            string displayText = string.Empty;

            if (observableVarInfoDO.IsCollection)
            {
                object currentValue = observableVarInfoDO.ValuePropertyInfo.GetValue(this.observableVarInstance);
                ICollection collection = currentValue as ICollection;
                displayText = $"(COLLECTION) {this.varName.Value} Count: {collection.Count}";

                if (this.observableVarInfoDO.isExpandedView.Value)
                {
                    int index = 0;
                    foreach (var item in collection)
                    {
                        displayText += $"\r\n{index}: {item}";
                        index++;
                    }
                }
            }
            else
            {
                if (observableVarInfoDO.ValuePropertyInfo == null) //Null here means ObservableVar is an Event
                {
                    displayText = $"EVENT: {varName.Value}";
                }
                else
                {
                    displayText = $"{varName.Value} : {this.observableVarInstance}";
                }
            }

            FieldInfo moderatorListFieldInfo = ObservableVarInfoDO.FieldType.GetField("moderators", bindingFlags);
            if (moderatorListFieldInfo != null)
            {
                IEnumerable moderators = moderatorListFieldInfo.GetValue(this.ObservableVarInstance) as IEnumerable;

                foreach (object moderatorListObj in moderators)
                {
                    PropertyInfo keyProp = moderatorListObj.GetType().GetProperty("Key");
                    int priority = (int)keyProp.GetValue(moderatorListObj);

                    PropertyInfo valProp = moderatorListObj.GetType().GetProperty("Value");
                    IEnumerable moderatorList = valProp.GetValue(moderatorListObj) as IEnumerable;

                    foreach (object moderator in moderatorList)
                    {
                        PropertyInfo methodProperty = moderator.GetType().GetProperty("Method", bindingFlags);
                        object moderatorMethodObj = methodProperty.GetValue(moderator);
                        PropertyInfo namePropertyInfo = moderatorMethodObj.GetType().GetProperty("Name", bindingFlags);
                        displayText += $"\nModerator {priority}: {namePropertyInfo.GetValue(moderatorMethodObj)}";
                    }
                }
            }

            this.cardText.Value = displayText;
        }
    }
}