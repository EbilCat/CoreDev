﻿using System.Collections.Generic;
using CoreDev.Extensions;
using CoreDev.Framework;
using UnityEngine;

public class InspectedDataObjectCardSpawner : MonoBehaviour
{
    [SerializeField] private InspectedDataObjectCard prefab;
    private Dictionary<InspectedDataObjectDO, InspectedDataObjectCard> inspectedDataObjectCards = new Dictionary<InspectedDataObjectDO, InspectedDataObjectCard>();


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
        InspectedDataObjectCard prefabInstance = Instantiate<InspectedDataObjectCard>(prefab);

        RectTransform rectTransform = prefabInstance.transform as RectTransform;
        rectTransform.SetParentAndZeroOutValues(this.transform, true);

        inspectedDataObjectCards.Add(inspectedDataObjectDO, prefabInstance);
        inspectedDataObjectDO.BindAspect(prefabInstance);
    }
    
    private void DataObjectDisposing(InspectedDataObjectDO inspectedDataObjectDO)
    {
        if (inspectedDataObjectCards.ContainsKey(inspectedDataObjectDO))
        {
            InspectedDataObjectCard prefabInstance = inspectedDataObjectCards[inspectedDataObjectDO];
            inspectedDataObjectDO.UnbindAspect(prefabInstance);
            this.inspectedDataObjectCards.Remove(inspectedDataObjectDO);
            Destroy(prefabInstance.gameObject);
        }
    }
}