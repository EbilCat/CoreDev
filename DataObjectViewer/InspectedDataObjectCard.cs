using System.Collections.Generic;
using System.Collections.ObjectModel;
using CoreDev.Extensions;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;


namespace CoreDev.DataObjectInspector
{
    public class InspectedDataObjectCard : MonoBehaviour, ISpawnee
    {
        [SerializeField] private InspectedObservableVarCard prefab;
        [SerializeField] private Transform content;
        private InspectedDataObjectDO inspectedDataObjectDO;
        private DataObjectInspectorDO dataObjectInspectorDO;
        private Dictionary<InspectedObservableVarDO, InspectedObservableVarCard> observableVarCards = new Dictionary<InspectedObservableVarDO, InspectedObservableVarCard>();


//*====================
//* UNITY
//*====================
        private void OnDestroy()
        {
            UnbindDO(this.inspectedDataObjectDO);
            UnbindDO(this.dataObjectInspectorDO);
        }


//*====================
//* BINDING
//*====================
        public void BindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedDataObjectDO && this.inspectedDataObjectDO == null)
            {
                this.inspectedDataObjectDO = dataObject as InspectedDataObjectDO;
                CompleteBinding();
            }

            if (dataObject is DataObjectInspectorDO && this.dataObjectInspectorDO == null)
            {
                this.dataObjectInspectorDO = dataObject as DataObjectInspectorDO;
                CompleteBinding();
            }
        }

        private void CompleteBinding()
        {
            if(this.inspectedDataObjectDO != null && this.dataObjectInspectorDO != null)
            {
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
                    dataObjectInspectorDO.BindAspect(prefabInstance);
                }

                this.inspectedDataObjectDO.isInspected.RegisterForChanges(OnIsInspectedChanged);
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedDataObjectDO && this.inspectedDataObjectDO == dataObject as InspectedDataObjectDO ||
                dataObject is DataObjectInspectorDO && this.dataObjectInspectorDO == dataObject as DataObjectInspectorDO
            )
            {
                ReadOnlyCollection<InspectedObservableVarDO> inspectedOVarDOs = inspectedDataObjectDO.inspectedOVarDOs.Value;

                for (int i = 0; i < inspectedOVarDOs.Count; i++)
                {
                    InspectedObservableVarDO inspectedObservableVarDO = inspectedOVarDOs[i];
                    InspectedObservableVarCard prefabInstance = observableVarCards[inspectedObservableVarDO];

                    observableVarCards.Remove(inspectedObservableVarDO);
                    inspectedObservableVarDO?.UnbindAspect(prefabInstance);
                    inspectedDataObjectDO?.UnbindAspect(prefabInstance);
                    dataObjectInspectorDO?.UnbindAspect(prefabInstance);

                    Destroy(prefabInstance.gameObject);
                }

                this.inspectedDataObjectDO?.isInspected.UnregisterFromChanges(OnIsInspectedChanged);
                this.inspectedDataObjectDO = null;

                this.dataObjectInspectorDO = null;
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
}