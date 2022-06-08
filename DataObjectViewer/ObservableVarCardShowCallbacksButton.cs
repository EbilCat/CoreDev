using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;
using UnityEngine.UI;


namespace CoreDev.DataObjectInspector
{
    public class ObservableVarCardShowCallbacksButton : MonoBehaviour, ISpawnee
    {
        private InspectedObservableVarDO inspectedObservableVarDO;
        private Image image;
        private Button button;


//*====================
//* BINDING
//*====================
        public void BindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedObservableVarDO)
            {
                UnbindDO(this.inspectedObservableVarDO);
                this.inspectedObservableVarDO = dataObject as InspectedObservableVarDO;

                this.image = this.GetComponent<Image>();
                this.button = this.GetComponent<Button>();
                this.button.onClick.AddListener(OnButtonClicked);

                this.inspectedObservableVarDO.showCallbacks.RegisterForChanges(OnShowCallbacksChanged);
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedObservableVarDO && this.inspectedObservableVarDO == dataObject)
            {
                this.button?.onClick.RemoveListener(OnButtonClicked);

                this.inspectedObservableVarDO?.showCallbacks.UnregisterFromChanges(OnShowCallbacksChanged);
                this.inspectedObservableVarDO = null;
            }
        }


//*====================
//* CALLBACKS - ObservableVarInfoDO
//*====================
        private void OnShowCallbacksChanged(ObservableVar<bool> oPrintToConsole)
        {
            this.image.color = (oPrintToConsole.Value) ? Color.green : Color.white;
        }


//*====================
//* PRIVATE
//*====================
        private void OnButtonClicked()
        {
            bool showCallbacks = this.inspectedObservableVarDO.showCallbacks.Value;
            this.inspectedObservableVarDO.showCallbacks.Value = !showCallbacks;
        }
    }
}