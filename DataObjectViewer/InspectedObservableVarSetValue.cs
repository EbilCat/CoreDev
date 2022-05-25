using System;
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
        private Dropdown dropDown;
        private InputField inputField;


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

                this.inspectedObservableVarDO.Focus.RegisterForChanges(FocusInputField, false);

                if (inspectedObservableVarDO.ObservableVarInfoDO.IsCollection)
                {
                    this.gameObject.SetActive(false);
                }
                else
                {
                    this.gameObject.SetActive(true);

                    this.dropDown = this.GetComponentInChildren<Dropdown>();
                    this.dropDown.options.Add(new Dropdown.OptionData("--Select Value--"));

                    this.inputField = this.GetComponentInChildren<InputField>();

                    this.inspectedObservableVarDO.ObservableVarInfoDO.isExpandedView.RegisterForChanges(OnIsExpandedViewChanged);

                    if (observableVarInfoDO.EnclosedValueType.IsEnum)
                    {
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
                        this.inputField.gameObject.SetActive(false);
                        this.dropDown.gameObject.SetActive(true);

                        this.dropDown.options.Add(new Dropdown.OptionData("True"));
                        this.dropDown.options.Add(new Dropdown.OptionData("False"));

                        this.dropDown.onValueChanged.AddListener(OnDropDownValueChanged);
                    }
                    else
                    {
                        this.dropDown.gameObject.SetActive(false);
                        this.inputField.gameObject.SetActive(true);
                        this.inputField.onEndEdit.AddListener(OnSubmit);
                    }
                }
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedObservableVarDO && this.inspectedObservableVarDO == dataObject)
            {
                this.inputField?.onEndEdit.RemoveListener(OnSubmit);

                this.inspectedObservableVarDO?.Focus.UnregisterFromChanges(FocusInputField);
                this.inspectedObservableVarDO?.ObservableVarInfoDO.isExpandedView.UnregisterFromChanges(OnIsExpandedViewChanged);
                this.inspectedObservableVarDO = null;
                this.observableVarInfoDO = null;
                this.observableVarInstance = null;
            }
        }


        //*====================
        //* CALLBACKS
        //*====================
        private void OnIsExpandedViewChanged(ObservableVar<bool> oIsExpandedView)
        {
            bool isExpandedView = oIsExpandedView.Value;
            if (isExpandedView)
            {
                this.inputField.text = observableVarInfoDO.GetValue(observableVarInstance)?.ToString();
                FocusInputField();
            }
        }

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

        private void OnDropDownValueChanged(int optionIndex)
        {
            if (optionIndex == 0) { return; }
            string text = this.dropDown.options[optionIndex].text;
            this.observableVarInfoDO.SetValue(observableVarInstance, text);
        }

        private void FocusInputField(object obj = null)
        {
            CoreDev.Sequencing.UniversalTimer.ScheduleCallback((x) =>
            {
                if (this.inputField.gameObject.activeInHierarchy)
                {
                    this.inputField.Select();
                    this.inputField.ActivateInputField();
                }

                if (this.dropDown.gameObject.activeInHierarchy)
                {
                    this.dropDown.Select();
                }
            });
        }
    }
}