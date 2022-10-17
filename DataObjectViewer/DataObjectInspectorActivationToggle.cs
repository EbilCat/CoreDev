using CoreDev.Framework;
using CoreDev.Observable;
using CoreDev.Sequencing;
using UnityEngine;

namespace CoreDev.DataObjectInspector
{
    public class DataObjectInspectorActivationToggle : MonoBehaviour, ISpawnee
    {
        [SerializeField] private KeyCode activationKey = KeyCode.F1;
        private DataObjectInspectorDO dataObjectInspectorDO;


        //*====================
        //* UNITY
        //*====================
        protected virtual void OnDestroy()
        {
            this.UnbindDO(this.dataObjectInspectorDO);
        }


        //*====================
        //* BINDING
        //*====================
        public void BindDO(IDataObject dataObject)
        {
            if (dataObject is DataObjectInspectorDO)
            {
                UnbindDO(this.dataObjectInspectorDO);

                this.dataObjectInspectorDO = dataObject as DataObjectInspectorDO;
                this.dataObjectInspectorDO.isOn.RegisterForChanges(OnIsOnChanged);

                UniversalTimer.RegisterForTimeElapsed(TimeElapsed);
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is DataObjectInspectorDO && this.dataObjectInspectorDO == (DataObjectInspectorDO)dataObject)
            {
                this.dataObjectInspectorDO?.isOn.UnregisterFromChanges(OnIsOnChanged);
                this.dataObjectInspectorDO = null;

                UniversalTimer.UnregisterFromTimeElapsed(TimeElapsed);
            }
        }


        //*====================
        //* IHasTimeElapsedHandler
        //*====================
        public void TimeElapsed(float deltaTime, float unscaledDeltaTime, int executionOrder)
        {
            if (Input.GetKeyDown(activationKey))
            {
                bool isInspectorOn = this.dataObjectInspectorDO.isOn.Value;
                this.dataObjectInspectorDO.isOn.Value = !isInspectorOn;
            }
        }


        //*====================
        //* CALLBACKS
        //*====================
        private void OnIsOnChanged(ObservableVar<bool> oIsOn)
        {
            bool isOn = oIsOn.Value;
            this.gameObject.SetActive(oIsOn.Value);
        }
    }
}