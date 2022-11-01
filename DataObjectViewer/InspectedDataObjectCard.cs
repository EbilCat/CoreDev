using System.Collections.Generic;
using System.Collections.ObjectModel;
using CoreDev.Extensions;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;


namespace CoreDev.DataObjectInspector
{
    public class InspectedDataObjectCard : BaseSpawnee
    {
        [SerializeField] private InspectedObservableVarCard prefab;
        [SerializeField] private Transform content;
        private InspectedDataObjectDO inspectedDataObjectDO;
        private Dictionary<InspectedObservableVarDO, InspectedObservableVarCard> observableVarCards = new Dictionary<InspectedObservableVarDO, InspectedObservableVarCard>();


//*====================
//* BINDING
//*====================
        public override void BindDO(IDataObject dataObject)
        {
            base.BindDO(dataObject);
            this.AttemptDependencyBind(dataObject, ref inspectedDataObjectDO);
        }

        public override void UnbindDO(IDataObject dataObject)
        {
            base.UnbindDO(dataObject);
            this.UnbindDependency(dataObject, ref inspectedDataObjectDO);
        }

        protected override bool FulfillDependencies()
        {
            bool fulfilled = base.FulfillDependencies();
            fulfilled &= inspectedDataObjectDO != null;
            return fulfilled;
        }

        protected override void ClearDependencies(object obj = null)
        {
            base.ClearDependencies(obj);
            this.ClearDependency(ref inspectedDataObjectDO);
        }

        protected override void RegisterCallbacks()
        {
            this.inspectedDataObjectDO.isInspected.RegisterForChanges(OnIsInspectedChanged);
            this.CreateObservableVarCards();
        }

        protected override void UnregisterCallbacks()
        {
            this.inspectedDataObjectDO?.isInspected.UnregisterFromChanges(OnIsInspectedChanged);
            this.DestroyObservableVarCards();
        }


//*====================
//* CALLBACKS - InspectedDataObjectDO
//*====================
        private void OnIsInspectedChanged(ObservableVar<bool> oIsInspected)
        {
            this.gameObject.SetActive(oIsInspected.Value);
        }


//*====================
//* PRIVATE
//*====================
        private void CreateObservableVarCards()
        {
            ReadOnlyCollection<InspectedObservableVarDO> inspectedObservableVarDOs = inspectedDataObjectDO.inspectedOVarDOs.Value;

            for (int i = 0; i < inspectedObservableVarDOs.Count; i++)
            {
                InspectedObservableVarDO inspectedObservableVarDO = inspectedObservableVarDOs[i];

                if (observableVarCards.ContainsKey(inspectedObservableVarDO) == false)
                {
                    InspectedObservableVarCard prefabInstance = Instantiate(prefab);

                    RectTransform rectTransform = prefabInstance.transform as RectTransform;
                    rectTransform.SetParentAndZeroOutValues(content, true);

                    observableVarCards.Add(inspectedObservableVarDO, prefabInstance);
                    inspectedObservableVarDO.BindAspect(prefabInstance);
                    inspectedDataObjectDO.BindAspect(prefabInstance);
                }
            }
        }

        private void DestroyObservableVarCards()
        {
            ReadOnlyCollection<InspectedObservableVarDO> inspectedObservableVarDOs = inspectedDataObjectDO.inspectedOVarDOs.Value;

            for (int i = 0; i < inspectedObservableVarDOs.Count; i++)
            {
                InspectedObservableVarDO inspectedObservableVarDO = inspectedObservableVarDOs[i];

                if (observableVarCards.ContainsKey(inspectedObservableVarDO))
                {
                    InspectedObservableVarCard prefabInstance = observableVarCards[inspectedObservableVarDO];

                    observableVarCards.Remove(inspectedObservableVarDO);
                    inspectedObservableVarDO?.UnbindAspect(prefabInstance);
                    inspectedDataObjectDO?.UnbindAspect(prefabInstance);

                    Destroy(prefabInstance.gameObject);
                }
            }
        }
    }
}