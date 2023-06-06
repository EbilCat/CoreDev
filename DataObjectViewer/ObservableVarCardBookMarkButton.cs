using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;
using UnityEngine.UI;


namespace CoreDev.DataObjectInspector
{
    public class ObservableVarCardBookMarkButton : MonoBehaviour, ISpawnee
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

                if(inspectedObservableVarDO.ObservableVarInfoDO.ValuePropertyInfo == null) //Implies is event thus bookmark does not apply
                {
                    this.gameObject.SetActive(false);
                    return;
                }

                this.image = this.GetComponent<Image>();
                this.button = this.GetComponent<Button>();
                this.button.onClick.RemoveListener(OnButtonClicked);
                this.button.onClick.AddListener(OnButtonClicked);

                this.inspectedObservableVarDO.ObservableVarInfoDO.isBookedMarked.RegisterForChanges(OnIsBookMarkedChanged);
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedObservableVarDO && this.inspectedObservableVarDO == dataObject)
            {
                this.button?.onClick.RemoveListener(OnButtonClicked);

                this.inspectedObservableVarDO?.ObservableVarInfoDO.isBookedMarked.UnregisterFromChanges(OnIsBookMarkedChanged);
                this.inspectedObservableVarDO = null;
            }
        }


//*====================
//* CALLBACKS - ObservableVarInfoDO
//*====================
        private void OnIsBookMarkedChanged(ObservableVar<bool> oIsBookMarked)
        {
            this.image.color = (oIsBookMarked.Value) ? Color.green : Color.white;
        }


//*====================
//* PRIVATE
//*====================
        private void OnButtonClicked()
        {
            bool isBookedMarked = this.inspectedObservableVarDO.ObservableVarInfoDO.isBookedMarked.Value;
            this.inspectedObservableVarDO.ObservableVarInfoDO.isBookedMarked.Value = !isBookedMarked;
        }
    }
}