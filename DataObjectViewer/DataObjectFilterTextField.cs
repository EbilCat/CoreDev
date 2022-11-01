using System.Collections.ObjectModel;
using CoreDev.Framework;
using CoreDev.Observable;
using CoreDev.Sequencing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace CoreDev.DataObjectInspector
{
    public class DataObjectFilterTextField : MonoBehaviour, ISpawnee
    {
        private DataObjectInspectorDO dataObjectInspectorDO;
        private InputField inputField;


//*====================
//* BINDING
//*====================
        public void BindDO(IDataObject dataObject)
        {
            if (dataObject is DataObjectInspectorDO)
            {
                this.inputField = this.GetComponent<InputField>();
                this.inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
                this.inputField.onEndEdit.AddListener(OnEndEdit);

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
                this.inputField.onEndEdit.RemoveListener(OnEndEdit);
                this.inputField = null;

                this.dataObjectInspectorDO?.isOn.UnregisterFromChanges(OnIsOnChanged);
                this.dataObjectInspectorDO = null;
            }
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

        private void OnEndEdit(string arg0)
        {
            if (Input.GetKeyDown(KeyCode.Return))
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
            else
            if (EventSystem.current.alreadySelecting == false)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}