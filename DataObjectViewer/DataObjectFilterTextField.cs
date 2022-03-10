using System;
using CoreDev.Framework;
using CoreDev.Observable;
using CoreDev.Sequencing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        this.dataObjectInspectorDO.filterString.Value = inputFieldValue;
    }

    private void OnEndEdit(string arg0)
    {
        if (EventSystem.current.alreadySelecting == false)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
