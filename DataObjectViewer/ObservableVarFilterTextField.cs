using CoreDev.Framework;
using CoreDev.Observable;
using CoreDev.Sequencing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


namespace CoreDev.DataObjectInspector
{
    public class ObservableVarFilterTextField : BaseSpawnee, ISelectHandler, IDeselectHandler
    {
        private InspectedDataObjectDO inspectedDataObjectDO;
        private TMP_InputField inputField;
        [SerializeField] private InputAction submit;


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
            fulfilled &= this.inputField = this.GetComponent<TMP_InputField>();
            return fulfilled;
        }

        protected override void ClearDependencies(object obj = null)
        {
            base.ClearDependencies(obj);
            this.ClearDependency(ref inspectedDataObjectDO);
            this.inputField = null;
        }

        protected override void RegisterCallbacks()
        {
            this.inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
            this.inspectedDataObjectDO.isInspected.RegisterForChanges(OnIsInspectedChanged);
            this.inspectedDataObjectDO.activateFilterTextField.RegisterForChanges(ActivateFilterTextField);
            this.inspectedDataObjectDO.observableVarFilterString.RegisterForChanges(OnObservableVarFilterStringChanged);
        }

        protected override void UnregisterCallbacks()
        {
            this.inputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
            this.inspectedDataObjectDO?.isInspected.UnregisterFromChanges(OnIsInspectedChanged);
            this.inspectedDataObjectDO?.activateFilterTextField.UnregisterFromChanges(ActivateFilterTextField);
            this.inspectedDataObjectDO?.observableVarFilterString.UnregisterFromChanges(OnObservableVarFilterStringChanged);

            this.submit.performed -= OnSubmitPerformed;
            this.submit.Disable();
        }


//*====================
//* ISelectHandler
//*====================
        public void OnSelect(BaseEventData eventData)
        {
            this.submit.Enable();
            this.submit.performed += OnSubmitPerformed;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            this.submit.performed -= OnSubmitPerformed;
            this.submit.Disable();
        }


//*====================
//* CALLBACKS
//*====================
        private void OnIsInspectedChanged(ObservableVar<bool> obj)
        {
            if (obj.Value)
            {
                this.ActivateFilterTextField();
            }
        }

        private void OnObservableVarFilterStringChanged(ObservableVar<string> obj)
        {
            this.inputField.text = obj.Value;
        }

        private void OnSubmitPerformed(InputAction.CallbackContext context)
        {
            this.Submit();
        }


//*====================
//* CALLBACKS - InputField
//*====================
        private void OnInputFieldValueChanged(string inputFieldValue)
        {
            this.inspectedDataObjectDO.observableVarFilterString.Value = inputFieldValue;
        }


//*====================
//* PUBLIC
//*====================
        public void Submit()
        {
            foreach (var inspectedOVar in inspectedDataObjectDO.inspectedOVarDOs)
            {
                if (inspectedOVar.matchesFilter.Value)
                {
                    inspectedOVar.ObservableVarInfoDO.isExpandedView.Value = true;
                    inspectedOVar.Focus.Fire();
                    break;
                }
            }
        }


//*====================
//* PRIVATE
//*====================
        private void ActivateFilterTextField(ObservableVar<object> obj = null)
        {
            //Have to delay a frame otherwise select won't work
            UniversalTimer.ScheduleCallback((x) =>
            {
                this.inputField?.Select();
                this.inputField?.ActivateInputField();
            });
        }
    }
}