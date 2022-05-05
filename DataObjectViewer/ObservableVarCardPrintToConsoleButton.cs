using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;
using UnityEngine.UI;


namespace CoreDev.DataObjectInspector
{
    public class ObservableVarCardPrintToConsoleButton : MonoBehaviour, ISpawnee
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

                this.inspectedObservableVarDO.printToConsole.RegisterForChanges(OnPrintToConsoleChanged);
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedObservableVarDO && this.inspectedObservableVarDO == dataObject)
            {
                this.button?.onClick.RemoveListener(OnButtonClicked);

                this.inspectedObservableVarDO?.printToConsole.UnregisterFromChanges(OnPrintToConsoleChanged);
                this.inspectedObservableVarDO = null;
            }
        }


//*====================
//* CALLBACKS - ObservableVarInfoDO
//*====================
        private void OnPrintToConsoleChanged(ObservableVar<bool> oPrintToConsole)
        {
            this.image.color = (oPrintToConsole.Value) ? Color.green : Color.white;
        }


//*====================
//* PRIVATE
//*====================
        private void OnButtonClicked()
        {
            bool printToConsole = this.inspectedObservableVarDO.printToConsole.Value;
            this.inspectedObservableVarDO.printToConsole.Value = !printToConsole;
        }
    }
}