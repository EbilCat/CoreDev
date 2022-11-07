using System.Text.RegularExpressions;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace CoreDev.DataObjectInspector
{
    public class InspectedObservableVarCard : BaseSpawnee, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler
    {
        private InspectedObservableVarDO inspectedObservableVarDO;
        private InspectedDataObjectDO inspectedDataObjectDO;
        private ObservableVarInfoDO observableVarInfoDO;
        private IObservableVar observableVarInstance;
        private RearrangeableScrollViewItem rearrangeableScrollViewItem;
        private Text text;


        //*====================
        //* BINDING
        //*====================
        public override void BindDO(IDataObject dataObject)
        {
            base.BindDO(dataObject);
            this.AttemptDependencyBind(dataObject, ref inspectedObservableVarDO);
            this.AttemptDependencyBind(dataObject, ref inspectedDataObjectDO);
        }

        public override void UnbindDO(IDataObject dataObject)
        {
            base.UnbindDO(dataObject);
            this.UnbindDependency(dataObject, ref inspectedObservableVarDO);
            this.UnbindDependency(dataObject, ref inspectedDataObjectDO);
        }

        protected override bool FulfillDependencies()
        {
            bool fulfilled = base.FulfillDependencies();
            fulfilled &= inspectedObservableVarDO != null;
            fulfilled &= inspectedDataObjectDO != null;
            fulfilled &= this.rearrangeableScrollViewItem = this.GetComponentInChildren<RearrangeableScrollViewItem>();
            fulfilled &= this.text = this.GetComponentInChildren<Text>();
            return fulfilled;
        }

        protected override void ClearDependencies(object obj = null)
        {
            base.ClearDependencies(obj);
            this.ClearDependency(ref inspectedObservableVarDO);
            this.ClearDependency(ref inspectedDataObjectDO);
            this.rearrangeableScrollViewItem = null;
            this.text = null;
        }

        protected override void RegisterCallbacks()
        {
            this.name = inspectedObservableVarDO.ObservableVarInfoDO.Name;
            this.observableVarInfoDO = inspectedObservableVarDO.ObservableVarInfoDO;
            this.observableVarInstance = inspectedObservableVarDO.ObservableVarInstance;

            this.rearrangeableScrollViewItem.RegisterForSiblingIndexChanged(OnSiblingIndexChanged);
            this.inspectedObservableVarDO.matchesFilter.RegisterForChanges(OnMatchesFilterChanged);
            this.inspectedObservableVarDO.cardText.RegisterForChanges(OnCardTextChanged);
            this.observableVarInfoDO.orderIndex.RegisterForChanges(OnOrderIndexChanged);
            this.inspectedDataObjectDO.observableVarFilterString.RegisterForChanges(OnObservableVarFilterString);
        }

        protected override void UnregisterCallbacks()
        {
            this.rearrangeableScrollViewItem?.UnregisterFromSiblingIndexChanged(OnSiblingIndexChanged);
            this.inspectedObservableVarDO?.matchesFilter.UnregisterFromChanges(OnMatchesFilterChanged);
            this.inspectedObservableVarDO?.cardText.UnregisterFromChanges(OnCardTextChanged);
            this.observableVarInfoDO?.orderIndex?.UnregisterFromChanges(OnOrderIndexChanged);
            this.inspectedDataObjectDO.observableVarFilterString?.UnregisterFromChanges(OnObservableVarFilterString);

            this.name = string.Empty;
            this.observableVarInfoDO = null;
            this.observableVarInstance = null;
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
                string observableVarFilterString = this.inspectedDataObjectDO.observableVarFilterString.Value;
                Match result = Regex.Match(this.text.text, observableVarFilterString, RegexOptions.Singleline | RegexOptions.IgnoreCase);
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
                bool isExpandedView = !this.inspectedObservableVarDO.ObservableVarInfoDO.isExpandedView.Value;
                this.inspectedObservableVarDO.ObservableVarInfoDO.isExpandedView.Value = isExpandedView;
                potentialClick = false;

                if (isExpandedView)
                {
                    inspectedObservableVarDO.Focus.Fire();
                }
            }
        }
    }
}