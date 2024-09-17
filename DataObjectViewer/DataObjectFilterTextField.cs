using System;
using System.Collections.ObjectModel;
using CoreDev.Framework;
using CoreDev.Observable;
using CoreDev.Sequencing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


namespace CoreDev.DataObjectInspector
{
    public class DataObjectFilterTextField : MonoBehaviour, ISpawnee, ISelectHandler, IDeselectHandler
    {
        private DataObjectInspectorDO dataObjectInspectorDO;
        private TMP_InputField inputField;
        [SerializeField] private InputAction submit;


//*====================
//* BINDING
//*====================
        public void BindDO(IDataObject dataObject)
        {
            if (dataObject is DataObjectInspectorDO)
            {
                this.inputField = this.GetComponent<TMP_InputField>();
                this.inputField.onValueChanged.AddListener(OnInputFieldValueChanged);

                UnbindDO(this.dataObjectInspectorDO);
                this.dataObjectInspectorDO = dataObject as DataObjectInspectorDO;
                this.dataObjectInspectorDO.isOn.RegisterForChanges(OnIsOnChanged);
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is DataObjectInspectorDO && this.dataObjectInspectorDO == (DataObjectInspectorDO)dataObject)
            {
                this.inputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
                this.inputField = null;

                this.dataObjectInspectorDO?.isOn.UnregisterFromChanges(OnIsOnChanged);
                this.dataObjectInspectorDO = null;

                this.submit.performed -= OnSubmitPerformed;
                this.submit.Disable();
            }
        }


//*====================
//* PUBLIC
//*====================
        public void Submit()
        {
            ReadOnlyCollection<InspectedDataObjectDO> inspectedDataObjectDOs = DataObjectInspectorMasterRepository.InspectedDataObjectDOs;
            foreach (var inspectedDataObjectDO in inspectedDataObjectDOs)
            {
                if (inspectedDataObjectDO.matchesFilter.Value)
                {
                    inspectedDataObjectDO.isInspected.Value = true;
                    inspectedDataObjectDO.activateFilterTextField.Fire();
                    break;
                }
            }
        }

//*====================
//* ISelectHandler
//*====================
        public void OnSelect(BaseEventData eventData)
        {
            this.submit.Enable();
            this.submit.performed += OnSubmitPerformed;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            this.submit.performed -= OnSubmitPerformed;
            this.submit.Disable();
        }


//*====================
//* CALLBACKS - DataObjectInspectorDO
//*====================
        private void OnIsOnChanged(ObservableVar<bool> oIsOn)
        {
            bool isOn = oIsOn.Value;

            if (isOn)
            {
                //Have to delay a frame otherwise select won't work
                UniversalTimer.ScheduleCallback((x) =>
                {
                    this.inputField.Select();
                    this.inputField.ActivateInputField();
                });
            }
        }


//*====================
//* CALLBACKS - InputField
//*====================
        private void OnInputFieldValueChanged(string inputFieldValue)
        {
            this.dataObjectInspectorDO.dataObjectFilterString.Value = inputFieldValue;
        }


//*====================
//* PRIVATE
//*====================
        private void OnSubmitPerformed(InputAction.CallbackContext context)
        {
            this.Submit();
        }
    }
}