using System.Collections.Generic;
using CoreDev.Framework;
using UnityEngine;


namespace CoreDev.DataObjectInspector
{
    public class DataObjectSelectionButtonSpawner : MonoBehaviour
    {
        [SerializeField] private DataObjectSelectionButton prefab;
        private Dictionary<IDataObject, DataObjectSelectionButton> dataObjectSelectionButtons = new Dictionary<IDataObject, DataObjectSelectionButton>();



        //*===========================
        //* UNITY
        //*===========================
        protected virtual void Awake()
        {
            DataObjectInspectorMasterRepository.RegisterForCreation(DataObjectCreated);
            DataObjectInspectorMasterRepository.RegisterForDisposing(DataObjectDisposing);
        }

        protected virtual void OnDestroy()
        {
            DataObjectInspectorMasterRepository.UnregisterFromCreation(DataObjectCreated);
            DataObjectInspectorMasterRepository.UnregisterFromDisposing(DataObjectDisposing);
        }


        //*====================
        //* BINDING
        //*====================
        private void DataObjectCreated(InspectedDataObjectDO inspectedDataObjectDO)
        {
            DataObjectSelectionButton prefabInstance = Instantiate(prefab);

            prefabInstance.transform.SetParent(this.transform);
            prefabInstance.transform.localPosition = Vector3.zero;
            prefabInstance.transform.localRotation = Quaternion.identity;
            prefabInstance.transform.localScale = Vector3.one;

            dataObjectSelectionButtons.Add(inspectedDataObjectDO, prefabInstance);
            inspectedDataObjectDO.BindAspect(prefabInstance);
        }

        private void DataObjectDisposing(InspectedDataObjectDO inspectedDataObjectDO)
        {
            if (dataObjectSelectionButtons.ContainsKey(inspectedDataObjectDO))
            {
                DataObjectSelectionButton prefabInstance = dataObjectSelectionButtons[inspectedDataObjectDO];
                inspectedDataObjectDO.UnbindAspect(prefabInstance);
                this.dataObjectSelectionButtons.Remove(inspectedDataObjectDO);
                Destroy(prefabInstance.gameObject);
            }
        }
    }
}