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
        public OAction Focus;

        public OString cardText;

        public InspectedObservableVarDO(InspectedDataObjectDO inspectedDataObject, IObservableVar observableVarInstance, ObservableVarInfoDO fieldInfoDO, OBool isInspected)
        {
            this.inspectedDataObject = inspectedDataObject;
            this.observableVarInstance = observableVarInstance;
            this.observableVarInfoDO = fieldInfoDO;
            this.isInspected = isInspected;
            this.matchesFilter = new OBool(true, this);
            this.printToConsole = new OBool(false, this);
            this.Focus = new OAction(this);
            this.cardText = new OString(string.Empty, this);

            this.varName = new OString(fieldInfoDO.Name, this);

            this.isInspected.RegisterForChanges(EvaluateEventSubscription);
            this.printToConsole.RegisterForChanges(EvaluateEventSubscription);
        }


        //*====================
        //* IDataObject
        //*====================
        public void Dispose()
        {
            this.isInspected.UnregisterFromChanges(EvaluateEventSubscription);
            this.printToConsole.UnregisterFromChanges(EvaluateEventSubscription);

            this.cardText.UnregisterFromChanges(OnCardTextChanged);
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
                this.observableVarInfoDO.RegisterForValueChanges(observableVarInstance, OnValueChanged);
                this.observableVarInfoDO.isExpandedView.RegisterForChanges(OnIsExpandedViewChanged);
                this.OnValueChanged();
            }
            else
            {
                this.observableVarInfoDO.UnregisterFromValueChanges(observableVarInstance, OnValueChanged);
                this.observableVarInfoDO?.isExpandedView.UnregisterFromChanges(OnIsExpandedViewChanged);
            }

            if (this.printToConsole.Value)
            {
                this.cardText.RegisterForChanges(OnCardTextChanged);
                this.observableVarInfoDO.RegisterForValueChangeBlocks(observableVarInstance, OnValueChangeBlocked);
                this.observableVarInfoDO.RegisterForModeratorsChanges(observableVarInstance, OnValueChanged);
            }
            else
            {
                this.cardText.UnregisterFromChanges(OnCardTextChanged);
                this.observableVarInfoDO.UnregisterFromValueChangeBlocks(observableVarInstance, OnValueChangeBlocked);
                this.observableVarInfoDO.UnregisterFromModeratorsChanges(observableVarInstance, OnValueChanged);
            }
        }

        private void OnIsExpandedViewChanged(ObservableVar<bool> obj)
        {
            this.OnValueChanged();
        }

        private void OnCardTextChanged(ObservableVar<string> obj)
        {
            Debug.LogFormat("[Frame {0}] {1} => {2}", Time.frameCount, this.inspectedDataObject.DataObjectInstance, obj.Value);
        }

        private void OnModeratorsChanged()
        {
            Debug.LogFormat("[Frame {0}] Moderator on {1} updated", Time.frameCount, observableVarInfoDO.Name);
        }

        private void OnValueChangeBlocked(string moderatorName)
        {
            Debug.LogFormat("[Frame {0}] Change to {1} blocked by Moderator: {2}", Time.frameCount, observableVarInfoDO.Name, moderatorName);
        }

        private void OnValueChanged()
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

            PropertyInfo propertyInfo = observableVarInfoDO.FieldType.GetProperty("Value");
            object currentValue = propertyInfo.GetValue(this.observableVarInstance);

            string displayText = string.Empty;

            if (observableVarInfoDO.IsCollection)
            {
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
                displayText = $"{varName.Value} : {this.observableVarInstance}";
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

            if (this.ObservableVarInfoDO.isExpandedView.Value)
            {
                string callbacks = $"{(this.observableVarInstance as IObservableVar).GetCallbacks()}";
                if (callbacks.Length > 0)
                {
                    displayText += $"\n{callbacks}";
                }
            }
            this.cardText.Value = displayText;
        }
    }
}