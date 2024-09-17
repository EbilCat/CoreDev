using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreDev.DataObjectInspector
{
    public class DataObjectInspectorActivationToggle : MonoBehaviour, ISpawnee
    {
        private DataObjectInspectorDO dataObjectInspectorDO;
        [SerializeField] private InputAction activationKey;

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
            
                activationKey.Enable();
                activationKey.performed += OnActivationKeyPerformed;
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is DataObjectInspectorDO && this.dataObjectInspectorDO == (DataObjectInspectorDO)dataObject)
            {
                this.dataObjectInspectorDO?.isOn.UnregisterFromChanges(OnIsOnChanged);
                this.dataObjectInspectorDO = null;
            
                activationKey.Disable();
                activationKey.performed -= OnActivationKeyPerformed;
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

        private void OnActivationKeyPerformed(InputAction.CallbackContext context)
        {
            bool isOn = this.dataObjectInspectorDO.isOn.Value;
            this.dataObjectInspectorDO.isOn.Value = !isOn;
        }
    }
}