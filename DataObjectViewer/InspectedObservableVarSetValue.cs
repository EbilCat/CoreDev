﻿using System;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace CoreDev.DataObjectInspector
{
    public class InspectedObservableVarSetValue : MonoBehaviour, ISpawnee
    {
        private InspectedObservableVarDO inspectedObservableVarDO;
        private ObservableVarInfoDO observableVarInfoDO;
        private IObservableVar observableVarInstance;
        [SerializeField] private Button fireEventButton;
        [SerializeField] private Dropdown dropDown;
        [SerializeField] private InputField inputField;


//*====================
//* BINDING
//*====================
        public void BindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedObservableVarDO)
            {
                UnbindDO(this.inspectedObservableVarDO);

                this.inspectedObservableVarDO = dataObject as InspectedObservableVarDO;
                this.observableVarInfoDO = inspectedObservableVarDO.ObservableVarInfoDO;
                this.observableVarInstance = inspectedObservableVarDO.ObservableVarInstance;

                this.inspectedObservableVarDO.Focus.RegisterForChanges(FocusInputField);

                this.dropDown.options.Add(new Dropdown.OptionData("--Select Value--"));

                if (observableVarInfoDO.FieldType == typeof(OEvent))
                {
                    this.fireEventButton.gameObject.SetActive(true);
                    this.inputField.gameObject.SetActive(true);
                    this.dropDown.gameObject.SetActive(false);

                    this.fireEventButton.onClick.AddListener(OnFireEventButtonClicked);
                    this.inputField.onEndEdit.AddListener(OnSubmit);
                }
                else
                if (observableVarInfoDO.EnclosedValueType.IsEnum)
                {
                    this.fireEventButton.gameObject.SetActive(false);
                    this.inputField.gameObject.SetActive(false);
                    this.dropDown.gameObject.SetActive(true);

                    string[] names = Enum.GetNames(observableVarInfoDO.EnclosedValueType);
                    for (int i = 0; i < names.Length; i++)
                    {
                        this.dropDown.options.Add(new Dropdown.OptionData(names[i]));
                    }
                    this.dropDown.onValueChanged.AddListener(OnDropDownValueChanged);
                }
                else
                if (observableVarInfoDO.EnclosedValueType == typeof(bool))
                {
                    this.fireEventButton.gameObject.SetActive(false);
                    this.inputField.gameObject.SetActive(false);
                    this.dropDown.gameObject.SetActive(true);

                    this.dropDown.options.Add(new Dropdown.OptionData("True"));
                    this.dropDown.options.Add(new Dropdown.OptionData("False"));

                    this.dropDown.onValueChanged.AddListener(OnDropDownValueChanged);
                }
                else
                {
                    this.fireEventButton.gameObject.SetActive(false);
                    this.dropDown.gameObject.SetActive(false);
                    this.inputField.gameObject.SetActive(true);
                    this.inputField.onEndEdit.AddListener(OnSubmit);
                }
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedObservableVarDO && this.inspectedObservableVarDO == dataObject)
            {
                this.inputField?.onEndEdit.RemoveListener(OnSubmit);

                this.inspectedObservableVarDO?.Focus.UnregisterFromChanges(FocusInputField);
                this.inspectedObservableVarDO = null;
                this.observableVarInfoDO = null;
                this.observableVarInstance = null;
            }
        }


//*====================
//* CALLBACKS
//*====================
        private void OnSubmit(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                this.observableVarInfoDO.SetValue(observableVarInstance, text);
                FocusInputField();
            }
            else
            {
                if (EventSystem.current.alreadySelecting == false)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }
            }
        }

        private void OnFireEventButtonClicked()
        {
            OEvent oEvent = this.observableVarInstance as OEvent;

            if (this.inputField.text.Length > 0)
            {
                oEvent.SetValueFromString(this.inputField.text);
            }
            else
            {
                oEvent.Fire();
            }
        }

        private void InspectDataObject()
        {
            IDataObject containedDataObject = this.observableVarInfoDO.GetValue(observableVarInstance) as IDataObject;

            if (containedDataObject != null)
            {
                InspectedDataObjectDO inspectedDataObjectDO = DataObjectInspectorMasterRepository.GetInspectedDataObjectDO(containedDataObject);
                inspectedDataObjectDO.isInspected.Value = true;
            }
        }

        private void OnDropDownValueChanged(int optionIndex)
        {
            if (optionIndex == 0) { return; }
            string text = this.dropDown.options[optionIndex].text;
            this.observableVarInfoDO.SetValue(observableVarInstance, text);
        }

        private void FocusInputField(ObservableVar<object> obj = null)
        {
            if (this.fireEventButton.gameObject.activeInHierarchy)
            {
                this.fireEventButton.Select();
            }

            if (this.inputField.gameObject.activeInHierarchy)
            {
                this.inputField.text = (inspectedObservableVarDO.ObservableVarInfoDO.IsCollection) ? string.Empty : this.observableVarInstance.ToString();
                this.inputField.Select();
                this.inputField.ActivateInputField();
            }

            if (this.dropDown.gameObject.activeInHierarchy)
            {
                this.dropDown.Select();
            }
        }
    }
}