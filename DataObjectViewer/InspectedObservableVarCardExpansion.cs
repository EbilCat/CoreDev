using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;


namespace CoreDev.DataObjectInspector
{
    public class InspectedObservableVarCardExpansion : MonoBehaviour, ISpawnee
    {
        private InspectedObservableVarDO inspectedObservableVarDO;
        private ObservableVarInfoDO observableVarInfoDO;


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

                this.observableVarInfoDO = inspectedObservableVarDO.ObservableVarInfoDO;
                this.observableVarInfoDO.isExpandedView.RegisterForChanges(OnIsExpandedViewChanged);
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedObservableVarDO && this.inspectedObservableVarDO == dataObject)
            {
                this.observableVarInfoDO?.isExpandedView.UnregisterFromChanges(OnIsExpandedViewChanged);

                this.observableVarInfoDO = null;
                this.inspectedObservableVarDO = null;
            }
        }


        //*====================
        //* CALLBACKS - InspectedObservableVarDO
        //*====================
        private void OnIsExpandedViewChanged(ObservableVar<bool> oIsExpandedView)
        {
            bool isExpandedView = oIsExpandedView.Value;
            this.gameObject.SetActive(isExpandedView);
        }
    }
}