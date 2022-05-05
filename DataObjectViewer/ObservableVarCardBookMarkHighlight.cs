using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;


namespace CoreDev.DataObjectInspector
{
    public class ObservableVarCardBookMarkHighlight : MonoBehaviour, ISpawnee
    {
        private InspectedObservableVarDO inspectedObservableVarDO;


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
            if (dataObject is InspectedObservableVarDO)
            {
                UnbindDO(this.inspectedObservableVarDO);
                this.inspectedObservableVarDO = dataObject as InspectedObservableVarDO;
                this.inspectedObservableVarDO.ObservableVarInfoDO.isBookedMarked.RegisterForChanges(OnIsBookedMarkedChanged);
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedObservableVarDO && this.inspectedObservableVarDO == dataObject as InspectedObservableVarDO)
            {
                this.inspectedObservableVarDO?.ObservableVarInfoDO.isBookedMarked.UnregisterFromChanges(OnIsBookedMarkedChanged);
                this.inspectedObservableVarDO = null;
            }
        }


//*====================
//* CALLBACKS - ObservableVarInfoDO 
//*====================
        private void OnIsBookedMarkedChanged(ObservableVar<bool> obj)
        {
            this.gameObject.SetActive(obj.Value);
        }
    }
}