using System;
using CoreDev.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace CoreDev.DataObjectInspector
{
    public class DataObjectInspectorCloseButton : MonoBehaviour, ISpawnee
    {
        private DataObjectInspectorDO dataObjectInspectorDO;
        private Button button;


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
            if(dataObject is DataObjectInspectorDO && this.dataObjectInspectorDO == null)
            {
                this.dataObjectInspectorDO = dataObject as DataObjectInspectorDO;
                this.button = this.GetComponent<Button>();
                this.button.onClick.RemoveListener(OnButtonClicked);
                this.button.onClick.AddListener(OnButtonClicked);
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if(dataObject is DataObjectInspectorDO && this.dataObjectInspectorDO == dataObject as DataObjectInspectorDO)
            {
                this.button.onClick.RemoveListener(OnButtonClicked);
                this.button = null;

                this.dataObjectInspectorDO = null;
            }
        }


//*====================
//* CALLBACKS - DataObjectInspectorDO
//*====================
        private void OnButtonClicked()
        {
            this.dataObjectInspectorDO.isOn.Value = false;
        }
    }
}