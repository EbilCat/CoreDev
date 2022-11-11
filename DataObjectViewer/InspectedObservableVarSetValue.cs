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
        private Button fireEventButton;
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
                this.fireEventButton = this.GetComponentInChildren<Button>();
                this.dropDown = this.GetComponentInChildren<Dropdown>();
                this.inputField = this.GetComponentInChildren<InputField>();

                this.inspectedObservableVarDO.Focus.RegisterForChanges(FocusInputField);

                if (inspectedObservableVarDO.ObservableVarInfoDO.IsCollection)
                {
                    this.gameObject.SetActive(false);
                }
                else
                {
                    this.gameObject.SetActive(true);
                    this.dropDown.options.Add(new Dropdown.OptionData("--Select Value--"));

                    if(observableVarInfoDO.ValuePropertyInfo == null)
                    {
                        this.fireEventButton.gameObject.SetActive(true);
                        this.inputField.gameObject.SetActive(false);
                        this.dropDown.gameObject.SetActive(false);

                        this.fireEventButton.onClick.AddListener(OnFireEventButtonClicked);
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
            (this.observableVarInstance as OEvent).Fire();
        }

        private void OnDropDownValueChanged(int optionIndex)
        {
            if (optionIndex == 0) { return; }
            string text = this.dropDown.options[optionIndex].text;
            this.observableVarInfoDO.SetValue(observableVarInstance, text);
        }

        private void FocusInputField()
        {
            if (this.fireEventButton.gameObject.activeInHierarchy)
            {
                this.fireEventButton.Select();
            }

            if (this.inputField.gameObject.activeInHierarchy)
            {
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