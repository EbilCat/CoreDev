﻿using System;
using CoreDev.Framework;
using CoreDev.Observable;
using CoreDev.Sequencing;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


namespace CoreDev.DataObjectInspector
{
    public class InspectedObservableVarSetValue : MonoBehaviour, ISpawnee
    {
        private InspectedObservableVarDO inspectedObservableVarDO;
        private ObservableVarInfoDO observableVarInfoDO;
        private IObservableVar observableVarInstance;
        [SerializeField] private Button fireEventButton;
        [SerializeField] private TMP_Dropdown dropDown;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button submitInputFieldButton;


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

                this.dropDown.options.Add(new TMP_Dropdown.OptionData("--Select Value--"));

                if (observableVarInfoDO.FieldType == typeof(OEvent))
                {
                    this.fireEventButton.gameObject.SetActive(true);
                    this.inputField.gameObject.SetActive(true);
                    this.submitInputFieldButton.gameObject.SetActive(false);
                    this.dropDown.gameObject.SetActive(false);

                    this.fireEventButton.onClick.AddListener(OnFireEventButtonClicked);
                }
                else
                if (observableVarInfoDO.EnclosedValueType.IsEnum)
                {
                    this.fireEventButton.gameObject.SetActive(false);
                    this.inputField.gameObject.SetActive(false);
                    this.submitInputFieldButton.gameObject.SetActive(false);
                    this.dropDown.gameObject.SetActive(true);

                    string[] names = Enum.GetNames(observableVarInfoDO.EnclosedValueType);
                    for (int i = 0; i < names.Length; i++)
                    {
                        this.dropDown.options.Add(new TMP_Dropdown.OptionData(names[i]));
                    }
                    this.dropDown.onValueChanged.AddListener(OnDropDownValueChanged);
                }
                else
                if (observableVarInfoDO.EnclosedValueType == typeof(bool))
                {
                    this.fireEventButton.gameObject.SetActive(false);
                    this.inputField.gameObject.SetActive(false);
                    this.submitInputFieldButton.gameObject.SetActive(false);
                    this.dropDown.gameObject.SetActive(true);

                    this.dropDown.options.Add(new TMP_Dropdown.OptionData("True"));
                    this.dropDown.options.Add(new TMP_Dropdown.OptionData("False"));

                    this.dropDown.onValueChanged.AddListener(OnDropDownValueChanged);
                }
                else
                {
                    this.fireEventButton.gameObject.SetActive(false);
                    this.dropDown.gameObject.SetActive(false);
                    this.inputField.gameObject.SetActive(true);
                    this.submitInputFieldButton.gameObject.SetActive(true);
                }
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedObservableVarDO && this.inspectedObservableVarDO == dataObject)
            {
                this.fireEventButton?.onClick.RemoveListener(OnFireEventButtonClicked);
                this.dropDown?.onValueChanged.RemoveListener(OnDropDownValueChanged);

                this.inspectedObservableVarDO?.Focus.UnregisterFromChanges(FocusInputField);
                this.inspectedObservableVarDO = null;
                this.observableVarInfoDO = null;
                this.observableVarInstance = null;
            }
        }


        //*====================
        //* PUBLIC
        //*====================
        public void Submit()
        {
            this.observableVarInfoDO.SetValue(observableVarInstance, this.inputField.text);
            this.FocusInputField();
        }


        //*====================
        //* CALLBACKS
        //*====================
        private void OnSubmitPerformed(InputAction.CallbackContext context)
        {
            this.Submit();
        }

        private void OnSubmitInputFieldButton()
        {
            this.observableVarInfoDO.SetValue(observableVarInstance, this.inputField.text);
            FocusInputField();
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
                //Have to delay a frame otherwise select won't work
                UniversalTimer.ScheduleCallback((x) =>
                {
                    this.dropDown.Select();
                });
            }
        }
    }
}