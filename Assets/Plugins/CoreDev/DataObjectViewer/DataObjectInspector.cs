using System.Text.RegularExpressions;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;


public class DataObjectInspector : MonoBehaviour, ISpawnee
{
    private DataObjectInspectorDO dataObjectInspectorDO;
    [SerializeField] private DataObjectSelectionButtonSpawner dataObjectSelectionButtonSpawner;
    [SerializeField] private InspectedDataObjectCardSpawner inspectedDataObjectCardSpawner;


//*====================
//* BINDING
//*====================
    public void BindDO(IDataObject dataObject)
    {
        if (dataObject is DataObjectInspectorDO)
        {
            UnbindDO(this.dataObjectInspectorDO);
            this.dataObjectInspectorDO = dataObject as DataObjectInspectorDO;
            this.dataObjectInspectorDO.isOn.RegisterForChanges(OnIsOnChanged);
        }
    }

    public void UnbindDO(IDataObject dataObject)
    {
        if (dataObject is DataObjectInspectorDO && this.dataObjectInspectorDO == dataObject as DataObjectInspectorDO)
        {
            this.dataObjectInspectorDO?.isOn.UnregisterFromChanges(OnIsOnChanged);

            this.dataObjectInspectorDO?.filterString.UnregisterFromChanges(OnFilterStringChanged);
            DataObjectMasterRepository.UnregisterFromCreation(OnDataObjectCreated);
            DataObjectMasterRepository.UnregisterFromDisposing(OnDataObjectDisposing);

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
            this.dataObjectInspectorDO.filterString.RegisterForChanges(OnFilterStringChanged);
            DataObjectMasterRepository.RegisterForCreation(OnDataObjectCreated);
            DataObjectMasterRepository.RegisterForDisposing(OnDataObjectDisposing);

            this.dataObjectInspectorDO?.isOn.UnregisterFromChanges(OnIsOnChanged);
        }
    }

    private void OnFilterStringChanged(ObservableVar<string> oFilterString)
    {
        string filterString = oFilterString.Value;

        foreach (InspectedDataObjectDO inspectedDataObjectDO in DataObjectInspectorMasterRepository.InspectedDataObjectDOs)
        {
            this.ApplyFilterCheck(filterString, inspectedDataObjectDO);
        }
    }

    private void OnDataObjectCreated(IDataObject dataObject)
    {
        InspectedDataObjectDO inspectedDataObject = new InspectedDataObjectDO(dataObject);
        DataObjectInspectorMasterRepository.RegisterInspectedDataObjectDO(dataObject, inspectedDataObject);

        inspectedDataObject.name.RegisterForChanges(OnNameChanged);
        inspectedDataObject.isInspected.RegisterForChanges(OnIsInspectedChanged);
    }

    private void OnDataObjectDisposing(IDataObject dataObject)
    {
        if (DataObjectInspectorMasterRepository.ContainsEntry(dataObject))
        {
            InspectedDataObjectDO inspectedDataObject = DataObjectInspectorMasterRepository.GetInspectedDataObjectDO(dataObject);
            inspectedDataObject.name.UnregisterFromChanges(OnNameChanged);
            inspectedDataObject.isInspected.UnregisterFromChanges(OnIsInspectedChanged);

            DataObjectInspectorMasterRepository.UnregisterInspectedDataObjectDO(dataObject);
        }
    }


//*====================
//* CALLBACKS - InspectedDataObjectDO
//*====================
    private void OnNameChanged(ObservableVar<string> oName)
    {
        ApplyFilterCheck(this.dataObjectInspectorDO.filterString.Value, (InspectedDataObjectDO)oName.DataObject);
    }


    private InspectedDataObjectDO currentSelectedInspectedDataObjectDO = null;
    private void OnIsInspectedChanged(ObservableVar<bool> oIsInspected)
    {
        bool isInspected = oIsInspected.Value;
        if (isInspected)
        {
            if (currentSelectedInspectedDataObjectDO != null)
            {
                this.currentSelectedInspectedDataObjectDO.isInspected.Value = false;
            }
            this.currentSelectedInspectedDataObjectDO = (InspectedDataObjectDO)oIsInspected.DataObject;
        }
    }


//*====================
//* PRIVATE
//*====================
    private void ApplyFilterCheck(string filterString, InspectedDataObjectDO inspectedDataObjectDO)
    {
        string inspectedDataObjectName = inspectedDataObjectDO.name.Value;
        try
        {
            Match result = Regex.Match(inspectedDataObjectName, filterString, RegexOptions.Singleline);
            inspectedDataObjectDO.matchesFilter.Value = result.Success;
        }
        catch
        {
            // Debug.LogWarning("Illegal Regex used in filter");
        }
    }
}