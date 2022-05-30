using System;
using System.Collections.ObjectModel;
using CoreDev.Framework;
using CoreDev.Observable;
using CoreDev.Sequencing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace CoreDev.DataObjectInspector
{
    public class ObservableVarFilterTextField : MonoBehaviour, ISpawnee
    {
        private DataObjectInspectorDO dataObjectInspectorDO;
        private InputField inputField;
        private bool wasFocusedLastFrame;


        //*====================
        //* BINDING
        //*====================
        public void BindDO(IDataObject dataObject)
        {
            if (dataObject is DataObjectInspectorDO && dataObjectInspectorDO == null)
            {
                this.dataObjectInspectorDO = dataObject as DataObjectInspectorDO;
                this.inputField = this.GetComponent<InputField>();

                this.inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
                this.inputField.onEndEdit.AddListener(OnEndEdit);

                this.dataObjectInspectorDO.dataObjectFilterSubmitted.RegisterForChanges(OnDataObjectFilterSubmitted);
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is DataObjectInspectorDO && this.dataObjectInspectorDO == (DataObjectInspectorDO)dataObject)
            {
                this.inputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
                this.inputField.onEndEdit.RemoveListener(OnEndEdit);
                this.inputField = null;

                this.dataObjectInspectorDO?.dataObjectFilterSubmitted.UnregisterFromChanges(OnDataObjectFilterSubmitted);
                this.dataObjectInspectorDO = null;
            }
        }

        private void OnDataObjectFilterSubmitted(ObservableVar<object> obj)
        {
            this.inputField.Select();
            this.inputField.ActivateInputField();
        }


        //*====================
        //* CALLBACKS - InputField
        //*====================
        private void OnInputFieldValueChanged(string inputFieldValue)
        {
            this.dataObjectInspectorDO.observableVarFilterString.Value = inputFieldValue;
        }

        private void OnEndEdit(string arg0)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                this.dataObjectInspectorDO?.observableVarFilterSubmitted.Fire();

                ReadOnlyCollection<InspectedDataObjectDO> inspectedDataObjectDOs = DataObjectInspectorMasterRepository.InspectedDataObjectDOs;
                foreach (var inspectedDataObjectDO in inspectedDataObjectDOs)
                {
                    if (inspectedDataObjectDO.isInspected.Value == true)
                    {
                        foreach (var inspectedOVar in inspectedDataObjectDO.inspectedOVarDOs)
                        {
                            if (inspectedOVar.matchesFilter.Value)
                            {
                                inspectedOVar.ObservableVarInfoDO.isExpandedView.Value = true;
                                inspectedOVar.Focus.Fire();
                                break;
                            }
                        }
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