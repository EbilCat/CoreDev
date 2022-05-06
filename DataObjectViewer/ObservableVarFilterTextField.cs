using CoreDev.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace CoreDev.DataObjectInspector
{
    public class ObservableVarFilterTextField : MonoBehaviour, ISpawnee
    {
        private DataObjectInspectorDO dataObjectInspectorDO;
        private InputField inputField;


//*====================
//* BINDING
//*====================
        public void BindDO(IDataObject dataObject)
        {
            if (dataObject is DataObjectInspectorDO && dataObjectInspectorDO == null)
            {
                this.dataObjectInspectorDO = dataObject as DataObjectInspectorDO;

                this.inputField = this.GetComponent<InputField>();
                this.inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
                this.inputField.onEndEdit.AddListener(OnEndEdit);
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is DataObjectInspectorDO && this.dataObjectInspectorDO == (DataObjectInspectorDO)dataObject)
            {
                this.inputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
                this.inputField.onEndEdit.RemoveListener(OnEndEdit);
                this.inputField = null;

                this.dataObjectInspectorDO = null;
            }
        }


//*====================
//* CALLBACKS - InputField
//*====================
        private void OnInputFieldValueChanged(string inputFieldValue)
        {
            this.dataObjectInspectorDO.observableVarFilterString.Value = inputFieldValue;
        }

        private void OnEndEdit(string arg0)
        {
            if (EventSystem.current.alreadySelecting == false)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}