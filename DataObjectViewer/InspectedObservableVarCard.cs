using System;
using System.Text.RegularExpressions;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace CoreDev.DataObjectInspector
{
    public class InspectedObservableVarCard : MonoBehaviour, ISpawnee, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler
    {
        private InspectedObservableVarDO inspectedObservableVarDO;
        private DataObjectInspectorDO dataObjectInspectorDO;
        private ObservableVarInfoDO observableVarInfoDO;
        private IObservableVar observableVarInstance;
        private RearrangeableScrollViewItem rearrangeableScrollViewItem;
        private Text text;


//*====================
//* UNITY
//*====================
        private void OnDestroy()
        {
            UnbindDO(this.inspectedObservableVarDO);
            UnbindDO(this.dataObjectInspectorDO);
        }


//*====================
//* BINDING
//*====================
        public void BindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedObservableVarDO && this.inspectedObservableVarDO == null)
            {
                this.inspectedObservableVarDO = dataObject as InspectedObservableVarDO;
                this.observableVarInfoDO = inspectedObservableVarDO.ObservableVarInfoDO;
                this.observableVarInstance = inspectedObservableVarDO.ObservableVarInstance;

                this.name = this.observableVarInfoDO.Name;
                this.rearrangeableScrollViewItem = this.GetComponentInChildren<RearrangeableScrollViewItem>();
                this.text = this.GetComponentInChildren<Text>();

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
            if (this.inspectedObservableVarDO != null && this.dataObjectInspectorDO != null)
            {
                this.rearrangeableScrollViewItem.RegisterForSiblingIndexChanged(OnSiblingIndexChanged);
                this.inspectedObservableVarDO.matchesFilter.RegisterForChanges(OnMatchesFilterChanged);
                this.inspectedObservableVarDO.cardText.RegisterForChanges(OnCardTextChanged);
                this.observableVarInfoDO.orderIndex.RegisterForChanges(OnOrderIndexChanged);
                this.dataObjectInspectorDO.observableVarFilterString.RegisterForChanges(OnObservableVarFilterString);
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedObservableVarDO && this.inspectedObservableVarDO == dataObject ||
                dataObject is DataObjectInspectorDO && this.dataObjectInspectorDO == dataObject as DataObjectInspectorDO)
            {
                this.rearrangeableScrollViewItem?.UnregisterFromSiblingIndexChanged(OnSiblingIndexChanged);
                this.inspectedObservableVarDO?.matchesFilter.UnregisterFromChanges(OnMatchesFilterChanged);
                this.inspectedObservableVarDO?.cardText.UnregisterFromChanges(OnCardTextChanged);
                this.observableVarInfoDO?.orderIndex?.UnregisterFromChanges(OnOrderIndexChanged);
                this.dataObjectInspectorDO.observableVarFilterString?.UnregisterFromChanges(OnObservableVarFilterString);

                this.rearrangeableScrollViewItem = null;
                this.observableVarInstance = null;

                this.inspectedObservableVarDO = null;
                this.observableVarInfoDO = null;
                this.dataObjectInspectorDO = null;
            }
        }


//*====================
//* CALLBACKS - RearrangeableScrollViewItem 
//*====================
        private void OnSiblingIndexChanged(int obj)
        {
            this.observableVarInfoDO.orderIndex.Value = obj;
        }


//*====================
//* CALLBACKS - ObservableVarInfoDO 
//*====================
        private void OnOrderIndexChanged(ObservableVar<int> obj)
        {
            this.transform.SetSiblingIndex(obj.Value);
            this.name = this.observableVarInfoDO.Name + ":" + this.observableVarInfoDO.orderIndex.Value + ":" + this.transform.GetSiblingIndex();
        }


//*====================
//* CALLBACKS
//*====================
        private void OnObservableVarFilterString(ObservableVar<string> obj)
        {
            try
            {
                string observableVarFilterString = this.dataObjectInspectorDO.observableVarFilterString.Value;
                Match result = Regex.Match(this.text.text, observableVarFilterString, RegexOptions.Singleline);
                this.inspectedObservableVarDO.matchesFilter.Value = result.Success;
            }
            catch
            {
                // Debug.LogWarning("Illegal Regex used in filter");
            }
        }

        private void OnMatchesFilterChanged(ObservableVar<bool> obj)
        {
            this.gameObject.SetActive(obj.Value);
        }

        private void OnCardTextChanged(ObservableVar<string> obj)
        {
            this.text.text = obj.Value;
        }


//*====================
//* IMPLEMENTATION - IPointerDownHandler
//*====================
        private bool potentialClick = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                potentialClick = true;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            potentialClick = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && potentialClick)
            {
                bool isExpandedView = this.inspectedObservableVarDO.ObservableVarInfoDO.isExpandedView.Value;
                this.inspectedObservableVarDO.ObservableVarInfoDO.isExpandedView.Value = !isExpandedView;
                potentialClick = false;
            }
        }
    }
}