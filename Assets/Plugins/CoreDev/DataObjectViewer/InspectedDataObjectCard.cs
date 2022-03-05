using System.Collections.Generic;
using System.Collections.ObjectModel;
using CoreDev.Extensions;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;

public class InspectedDataObjectCard : MonoBehaviour, ISpawnee
{
    [SerializeField] private InspectedObservableVarCard prefab;
    [SerializeField] private Transform content;
    private InspectedDataObjectDO inspectedDataObjectDO;
    private Dictionary<InspectedObservableVarDO, InspectedObservableVarCard> observableVarCards = new Dictionary<InspectedObservableVarDO, InspectedObservableVarCard>();


//*====================
//* BINDING
//*====================
    public void BindDO(IDataObject dataObject)
    {
        if(dataObject is InspectedDataObjectDO)
        {
            UnbindDO(this.inspectedDataObjectDO);
            this.inspectedDataObjectDO = dataObject as InspectedDataObjectDO;

            ReadOnlyCollection<InspectedObservableVarDO> inspectedObservableVarDOs = inspectedDataObjectDO.inspectedOVarDOs.Value;

            for (int i = 0; i < inspectedObservableVarDOs.Count; i++)
            {
                InspectedObservableVarDO inspectedObservableVarDO = inspectedObservableVarDOs[i];
                InspectedObservableVarCard prefabInstance = Instantiate(prefab);
                
                RectTransform rectTransform = prefabInstance.transform as RectTransform;
                rectTransform.SetParentAndZeroOutValues(content, true);

                observableVarCards.Add(inspectedObservableVarDO, prefabInstance);
                inspectedObservableVarDO.BindAspect(prefabInstance);
                inspectedDataObjectDO.BindAspect(prefabInstance);
            }

            this.inspectedDataObjectDO.isInspected.RegisterForChanges(OnIsInspectedChanged);
        }
    }

    public void UnbindDO(IDataObject dataObject)
    {
        if(dataObject is InspectedDataObjectDO && this.inspectedDataObjectDO == dataObject)
        {
            ReadOnlyCollection<InspectedObservableVarDO> inspectedOVarDOs = inspectedDataObjectDO.inspectedOVarDOs.Value;

            for (int i = 0; i < inspectedOVarDOs.Count; i++)
            {
                InspectedObservableVarDO inspectedObservableVarDO = inspectedOVarDOs[i];
                InspectedObservableVarCard prefabInstance = observableVarCards[inspectedObservableVarDO];

                observableVarCards.Remove(inspectedObservableVarDO);
                inspectedObservableVarDO.UnbindAspect(prefabInstance);
                inspectedDataObjectDO.UnbindAspect(prefabInstance);

                Destroy(prefabInstance.gameObject);
            }

            this.inspectedDataObjectDO?.isInspected.UnregisterFromChanges(OnIsInspectedChanged);
            this.inspectedDataObjectDO = null;
        }
    }


//*====================
//* CALLBACKS - InspectedDataObjectDO
//*====================
    private void OnIsInspectedChanged(ObservableVar<bool> oIsInspected)
    {
        this.gameObject.SetActive(oIsInspected.Value);
    }
}