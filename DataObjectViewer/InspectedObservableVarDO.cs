using System;
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
        public OString basicText;
        public OString expandedText;

        public OString varName;
        public OBool printToConsole;
        public OBool showCallbacks;
        public OEvent Focus;

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
            this.basicText = new OString(string.Empty, this);
            this.expandedText = new OString(string.Empty, this);

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

            observableVarInfoDO.UnregisterFromValueChanges(observableVarInstance, OnValueChanged);
            observableVarInfoDO.UnregisterFromValueChangeBlocks(observableVarInstance, OnValueChangeBlocked);
            observableVarInfoDO.UnregisterFromModeratorsChanges(observableVarInstance, OnModeratorsChanged);
        }


        //*====================
        //* CALLBACKS
        //*====================
        private void EvaluateEventSubscription(ObservableVar<bool> obj)
        {
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
    }
}