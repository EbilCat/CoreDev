using System.Collections;
using System.Reflection;
using CoreDev.Framework;
using CoreDev.Observable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoreDev.DataObjectInspector
{
    public class InspectedObservableVarCardBasicText : BaseSpawnee
    {
        private InspectedObservableVarDO inspectedObservableVarDO;
        private InspectedDataObjectDO inspectedDataObjectDO;
        private ObservableVarInfoDO observableVarInfoDO;
        private IObservableVar observableVarInstance;
        [SerializeField] private TextMeshProUGUI basicText;
        [SerializeField] private Button inspectButton;


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

            this.inspectedObservableVarDO.isInspected.RegisterForChanges(OnIsInspectedChanged);
            this.inspectedObservableVarDO.basicText.RegisterForChanges(OnCardTextChanged);
        }

        protected override void UnregisterCallbacks()
        {
            this.inspectedObservableVarDO?.isInspected.UnregisterFromChanges(OnIsInspectedChanged);
            this.inspectedObservableVarDO?.basicText.UnregisterFromChanges(OnCardTextChanged);

            this.observableVarInfoDO?.UnregisterFromValueChanges(observableVarInstance, RefreshCardText);
            this.observableVarInfoDO?.UnregisterFromValueChanges(observableVarInstance, EvaluateInspectButton);
            this.observableVarInfoDO?.UnregisterFromModeratorsChanges(observableVarInstance, RefreshCardText);

            this.name = string.Empty;
            this.inspectedDataObjectDO = null;
            this.observableVarInfoDO = null;
            this.observableVarInstance = null;
        }


//*====================
//* CALLBACKS
//*====================
        private void OnIsInspectedChanged(ObservableVar<bool> obj)
        {
            if (obj.Value)
            {
                this.observableVarInfoDO.RegisterForValueChanges(observableVarInstance, RefreshCardText, false);
                this.observableVarInfoDO.RegisterForValueChanges(observableVarInstance, EvaluateInspectButton);
                this.observableVarInfoDO.RegisterForModeratorsChanges(observableVarInstance, RefreshCardText);
            }
            else
            {
                this.observableVarInfoDO?.UnregisterFromValueChanges(observableVarInstance, RefreshCardText);
                this.observableVarInfoDO?.UnregisterFromValueChanges(observableVarInstance, EvaluateInspectButton);
                this.observableVarInfoDO?.UnregisterFromModeratorsChanges(observableVarInstance, RefreshCardText);
            }
        }

        private void OnCardTextChanged(ObservableVar<string> obj)
        {
            this.basicText.text = obj.Value;
        }

        private void OnInspectButtonClicked()
        {
            IDataObject dataObject = this.observableVarInfoDO?.GetValue(this.observableVarInstance) as IDataObject;
            InspectedDataObjectDO enclosedInspectedDataObjectDO = DataObjectInspectorMasterRepository.GetInspectedDataObjectDO(dataObject);
            if (enclosedInspectedDataObjectDO != null)
            {
                enclosedInspectedDataObjectDO.isInspected.Value = true;
            }
            else
            {
                InspectedDataObjectDO inspectedDataObjectDO = new InspectedDataObjectDO(dataObject);
                inspectedDataObjectDO.isInspected.Value = true;
                inspectedDataObjectDO.Inspect();
                InspectedDataObjectCardSpawner.Singleton.Inspect(inspectedDataObjectDO);
            }
        }


//*====================
//* PRIVATE
//*====================
        private void RefreshCardText()
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

            string displayText = string.Empty;

            if (observableVarInfoDO.IsCollection)
            {
                object currentValue = observableVarInfoDO.ValuePropertyInfo.GetValue(this.observableVarInstance);
                ICollection collection = currentValue as ICollection;
                displayText = $"(COLLECTION) {this.inspectedObservableVarDO.varName.Value} Count: {collection.Count}";
            }
            else
            {
                if (observableVarInstance is OEvent)
                {
                    displayText = $"EVENT: {inspectedObservableVarDO.varName.Value}";
                }
                else
                {
                    displayText = $"{inspectedObservableVarDO.varName.Value} : {this.observableVarInstance}";
                }
            }

            FieldInfo moderatorListFieldInfo = observableVarInfoDO.FieldType.GetField("moderators", bindingFlags);
            if (moderatorListFieldInfo != null)
            {
                IEnumerable moderators = moderatorListFieldInfo.GetValue(this.observableVarInstance) as IEnumerable;

                foreach (object moderatorListObj in moderators)
                {
                    PropertyInfo keyProp = moderatorListObj.GetType().GetProperty("Key");
                    int priority = (int)keyProp.GetValue(moderatorListObj);

                    PropertyInfo valProp = moderatorListObj.GetType().GetProperty("Value");
                    IEnumerable moderatorList = valProp.GetValue(moderatorListObj) as IEnumerable;

                    foreach (object moderator in moderatorList)
                    {
                        PropertyInfo methodProperty = moderator.GetType().GetProperty("Method", bindingFlags);
                        object moderatorMethodObj = methodProperty.GetValue(moderator);
                        PropertyInfo namePropertyInfo = moderatorMethodObj.GetType().GetProperty("Name", bindingFlags);
                        displayText += $"\nModerator {priority}: {namePropertyInfo.GetValue(moderatorMethodObj)}";
                    }
                }
            }

            this.inspectedObservableVarDO.basicText.Value = displayText;
        }

        private void EvaluateInspectButton()
        {
            this.inspectButton.onClick.RemoveAllListeners();
            IDataObject dataObject = this.observableVarInfoDO?.GetValue(this.observableVarInstance) as IDataObject;

            if (dataObject != null)
            {
                this.inspectButton.gameObject.SetActive(true);
                this.inspectButton.onClick.AddListener(OnInspectButtonClicked);
            }
            else
            {
                this.inspectButton.gameObject.SetActive(false);
                this.inspectButton.onClick.RemoveListener(OnInspectButtonClicked);
            }
        }
    }
}