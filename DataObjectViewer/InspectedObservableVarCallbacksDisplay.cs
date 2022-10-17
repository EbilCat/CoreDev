using System.Collections.Generic;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;
using UnityEngine.UI;

namespace CoreDev.DataObjectInspector
{
    public class InspectedObservableVarCallbacksDisplay : MonoBehaviour, ISpawnee
    {
        private InspectedObservableVarDO inspectedObservableVarDO;
        [SerializeField] private InputField callbackFilter;
        [SerializeField] private Text text;


//*====================
//* UNITY
//*====================
        protected virtual void OnDestroy()
        {
            this.UnbindDO(this.inspectedObservableVarDO);
        }


//*====================
//* BINDING
//*====================
        public void BindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedObservableVarDO && this.inspectedObservableVarDO == null)
            {
                this.inspectedObservableVarDO = dataObject as InspectedObservableVarDO;

                this.inspectedObservableVarDO.showCallbacks.RegisterForChanges(OnShowCallbacksChanged);
                this.callbackFilter.onValueChanged.AddListener(OnValueChanged);
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedObservableVarDO && this.inspectedObservableVarDO == dataObject as InspectedObservableVarDO)
            {
                this.inspectedObservableVarDO?.showCallbacks.UnregisterFromChanges(OnShowCallbacksChanged);
                this.callbackFilter.onValueChanged.RemoveListener(OnValueChanged);
                
                this.inspectedObservableVarDO.ObservableVarInfoDO.UnregisterFromCallbackChanges(this.inspectedObservableVarDO.ObservableVarInstance, RefreshCallbacksDisplay);

                this.inspectedObservableVarDO = null;
            }
        }


//*====================
//* CALLBACKS
//*====================
        private void OnValueChanged(string arg0)
        {
            RefreshCallbacksDisplay();
        }

        private void OnShowCallbacksChanged(ObservableVar<bool> obj)
        {
            if (obj.Value)
            {
                this.inspectedObservableVarDO.ObservableVarInfoDO.RegisterForCallbackChanges(this.inspectedObservableVarDO.ObservableVarInstance, RefreshCallbacksDisplay);
                this.gameObject.SetActive(true);
                this.RefreshCallbacksDisplay();
                CoreDev.Sequencing.UniversalTimer.ScheduleCallbackUnscaled((x) =>
                {
                    this.callbackFilter.Select();
                    this.callbackFilter.ActivateInputField();
                });
            }
            else
            {
                this.inspectedObservableVarDO.ObservableVarInfoDO.UnregisterFromCallbackChanges(this.inspectedObservableVarDO.ObservableVarInstance, RefreshCallbacksDisplay);
                this.gameObject.SetActive(false);
                this.text.text = string.Empty;
            }
        }

        List<string> callbacks = new List<string>();

        private void RefreshCallbacksDisplay()
        {
            IObservableVar ovar = this.inspectedObservableVarDO.ObservableVarInstance as IObservableVar;
            ovar.GetCallbacks(callbacks);

            string text = string.Empty;
            foreach (string callback in callbacks)
            {
                string filterText = callbackFilter.text;
                if (callback.Contains(callbackFilter.text) || filterText.Length == 0)
                {
                    text += $"\t{callback}\r\n";
                }
            }
            this.text.text = (text.Length == 0) ? "\tNONE" : text.TrimEnd();
        }
    }
}