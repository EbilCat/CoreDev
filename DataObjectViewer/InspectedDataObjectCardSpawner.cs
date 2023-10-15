using System.Collections.Generic;
using CoreDev.Extensions;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;


namespace CoreDev.DataObjectInspector
{
    public class InspectedDataObjectCardSpawner : MonoBehaviour
    {
        private static InspectedDataObjectCardSpawner singleton;
        public static InspectedDataObjectCardSpawner Singleton => singleton;

        [SerializeField] private InspectedDataObjectCard prefab;
        private Dictionary<InspectedDataObjectDO, InspectedDataObjectCard> inspectedDataObjectCards = new Dictionary<InspectedDataObjectDO, InspectedDataObjectCard>();

        protected void Awake()
        {
            if (singleton == null)
            {
                singleton = this;
                DataObjectInspectorMasterRepository.RegisterForCreation(DataObjectCreated);
                DataObjectInspectorMasterRepository.RegisterForDisposing(DataObjectDisposing);
            }
        }

        protected void OnDestroy()
        {
            DataObjectInspectorMasterRepository.UnregisterFromCreation(DataObjectCreated);
            DataObjectInspectorMasterRepository.UnregisterFromDisposing(DataObjectDisposing);
        }


//*====================
//* CALLBACKS - DataObjectInspectorMasterRepository
//*====================
        private void DataObjectCreated(InspectedDataObjectDO inspectedDataObjectDO)
        {
            inspectedDataObjectDO.isInspected.RegisterForChanges(OnIsInspectedChanged);
        }

        private void DataObjectDisposing(InspectedDataObjectDO inspectedDataObjectDO)
        {
            inspectedDataObjectDO?.isInspected.UnregisterFromChanges(OnIsInspectedChanged);

            this.DisposePrefab(inspectedDataObjectDO);
        }


//*====================
//* CALLBACKS - InspectedDataObjectDO
//*====================
        private void OnIsInspectedChanged(ObservableVar<bool> obj)
        {
            if (obj.DataObject is InspectedDataObjectDO)
            {
                InspectedDataObjectDO inspectedDataObjectDO = obj.DataObject as InspectedDataObjectDO;

                if (obj.Value == true)
                {
                    inspectedDataObjectDO.Inspect();
                    InstantiatePrefab(inspectedDataObjectDO);
                }
                else
                {
                    DisposePrefab(inspectedDataObjectDO);
                }
            }
        }


//*====================
//* PUBLIC
//*====================
        public void Inspect(InspectedDataObjectDO inspectedDataObjectDO)
        {
            InspectedDataObjectCard prefabInstance = Instantiate<InspectedDataObjectCard>(prefab);

            RectTransform rectTransform = prefabInstance.transform as RectTransform;
            rectTransform.SetParentAndZeroOutValues(this.transform, true);

            inspectedDataObjectCards.Add(inspectedDataObjectDO, prefabInstance);
            inspectedDataObjectDO.BindAspect(prefabInstance);
        }


//*====================
//* PRIVATE
//*====================
        private void InstantiatePrefab(InspectedDataObjectDO inspectedDataObjectDO)
        {
            Inspect(inspectedDataObjectDO);
        }

        private void DisposePrefab(InspectedDataObjectDO inspectedDataObjectDO)
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
}