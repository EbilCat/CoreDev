using System.Collections;
using System.Collections.Generic;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;

namespace CoreDev.DataObjectInspector
{
    public class InspectedObservableVarCardValueDisplayManager : BaseSpawnee
    {
        [SerializeField] private InspectedObservableVarCardCollectionItem collectionItemPrefab;
        private List<InspectedObservableVarCardCollectionItem> collectionItems = new List<InspectedObservableVarCardCollectionItem>();
        private InspectedObservableVarDO inspectedObservableVarDO;
        private InspectedDataObjectDO inspectedDataObjectDO;
        private ObservableVarInfoDO observableVarInfoDO;
        private IObservableVar observableVarInstance;


//*====================
//* BINDING
//*====================
        public override void BindDO(IDataObject dataObject)
        {
            base.BindDO(dataObject);
            this.AttemptDependencyBind(dataObject, ref inspectedObservableVarDO);
        }

        public override void UnbindDO(IDataObject dataObject)
        {
            base.UnbindDO(dataObject);
            this.UnbindDependency(dataObject, ref inspectedObservableVarDO);
        }

        protected override bool FulfillDependencies()
        {
            bool fulfilled = base.FulfillDependencies();
            fulfilled &= inspectedObservableVarDO != null;
            return fulfilled;
        }

        protected override void ClearDependencies(object obj = null)
        {
            base.ClearDependencies(obj);
            this.ClearDependency(ref inspectedObservableVarDO);
        }

        protected override void RegisterCallbacks()
        {
            this.name = inspectedObservableVarDO.ObservableVarInfoDO.Name;
            this.inspectedDataObjectDO = inspectedObservableVarDO.inspectedDataObject;
            this.observableVarInfoDO = inspectedObservableVarDO.ObservableVarInfoDO;
            this.observableVarInstance = inspectedObservableVarDO.ObservableVarInstance;

            if (this.observableVarInfoDO.IsCollection)
            {
                this.observableVarInfoDO.isExpandedView.RegisterForChanges(OnExpandedChanged);
            }
        }

        protected override void UnregisterCallbacks()
        {
            if (this.observableVarInfoDO.IsCollection)
            {
                this.observableVarInfoDO?.isExpandedView.UnregisterFromChanges(OnExpandedChanged);
            }
            
            this.observableVarInfoDO?.UnregisterFromValueChanges(observableVarInstance, RefreshList);

            this.name = string.Empty;
            this.inspectedDataObjectDO = null;
            this.observableVarInfoDO = null;
            this.observableVarInstance = null;
        }


//*====================
//* CALLBACKS
//*====================
        private void OnExpandedChanged(ObservableVar<bool> obj)
        {
            if (obj.Value)
            {
                this.observableVarInfoDO.RegisterForValueChanges(observableVarInstance, RefreshList);
            }
            else
            {
                this.observableVarInfoDO?.UnregisterFromValueChanges(observableVarInstance, RefreshList);

                for (int i = 0; i < collectionItems.Count; i++)
                {
                    collectionItems[i].gameObject.SetActive(false);
                }
                this.inspectedObservableVarDO.expandedText.Value = string.Empty;
            }
        }

        private void RefreshList()
        {
            object currentValue = observableVarInfoDO.ValuePropertyInfo.GetValue(this.observableVarInstance);
            ICollection collection = currentValue as ICollection;

            string expandedText = string.Empty;

            int index = 0;
            foreach (object obj in collection)
            {
                InspectedObservableVarCardCollectionItem collectionItem = null;

                if(collectionItems.Count > index)
                {
                    collectionItem = collectionItems[index];
                    collectionItem.gameObject.SetActive(true);
                }
                else
                {
                    collectionItem = Instantiate(collectionItemPrefab);
                    collectionItem.transform.SetParent(this.transform);
                    collectionItem.transform.localPosition = Vector3.zero;
                    collectionItem.transform.localRotation = Quaternion.identity;
                    collectionItem.transform.localScale = Vector3.one;
                    this.collectionItems.Add(collectionItem);
                }

                string displayText = collectionItem.SetCollectionItem(index, obj);
                expandedText += $"{displayText}\r\n";
                index++;
            }

            this.inspectedObservableVarDO.expandedText.Value = expandedText;

            for (int i = index; i < collectionItems.Count; i++)
            {
                collectionItems[i].gameObject.SetActive(false);
            }
        }
    }
}