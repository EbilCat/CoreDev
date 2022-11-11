using CoreDev.Framework;
using CoreDev.Observable;
using CoreDev.Sequencing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace CoreDev.DataObjectInspector
{
    public class ObservableVarFilterTextField : BaseSpawnee
    {
        private InspectedDataObjectDO inspectedDataObjectDO;
        private InputField inputField;


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
            fulfilled &= this.inputField = this.GetComponent<InputField>();
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
            this.inputField.onEndEdit.AddListener(OnEndEdit);
            this.inspectedDataObjectDO.isInspected.RegisterForChanges(OnIsInspectedChanged);
            this.inspectedDataObjectDO.activateFilterTextField.RegisterForChanges(ActivateFilterTextField);
            this.inspectedDataObjectDO.observableVarFilterString.RegisterForChanges(OnObservableVarFilterStringChanged);
        }

        protected override void UnregisterCallbacks()
        {
            this.inputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
            this.inputField.onEndEdit.RemoveListener(OnEndEdit);
            this.inspectedDataObjectDO?.isInspected.UnregisterFromChanges(OnIsInspectedChanged);
            this.inspectedDataObjectDO?.activateFilterTextField.UnregisterFromChanges(ActivateFilterTextField);
            this.inspectedDataObjectDO?.observableVarFilterString.UnregisterFromChanges(OnObservableVarFilterStringChanged);
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


//*====================
//* CALLBACKS - InputField
//*====================
        private void OnInputFieldValueChanged(string inputFieldValue)
        {
            this.inspectedDataObjectDO.observableVarFilterString.Value = inputFieldValue;
        }

        private void OnEndEdit(string arg0)
        {
            if (Input.GetKeyDown(KeyCode.Return))
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
            else
            if (EventSystem.current.alreadySelecting == false)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }


//*====================
//* PRIVATE
//*====================
        private void ActivateFilterTextField()
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