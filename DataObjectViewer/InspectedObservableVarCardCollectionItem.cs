using System;
using System.Reflection;
using CoreDev.Framework;
using UnityEngine;
using UnityEngine.UI;


namespace CoreDev.DataObjectInspector
{
    public class InspectedObservableVarCardCollectionItem : MonoBehaviour
    {
        [SerializeField] private Text text;
        [SerializeField] private Button inspectKeyButton;
        [SerializeField] private Button inspectValueButton;

        private InspectedDataObjectDO inspectedKeyDO = null;
        private InspectedDataObjectDO inspectedValDO = null;


//*====================
//* UNITY
//*====================
        private void Awake()
        {
            inspectKeyButton.onClick.AddListener(() => inspectedKeyDO.isInspected.Value = true );
            inspectValueButton.onClick.AddListener(() => inspectedValDO.isInspected.Value = true );
        }


//*====================
//* PUBLIC
//*====================
        public string SetCollectionItem(int index, object obj)
        {
            Type objType = obj?.GetType();
            PropertyInfo keyPropertyInfo = objType?.GetProperty("Key");
            PropertyInfo valPropertyInfo = objType?.GetProperty("Value");
            this.inspectedKeyDO = null;
            this.inspectedValDO = null;

            if (obj is IDataObject dataObject)
            {
                this.inspectedValDO = DataObjectInspectorMasterRepository.GetInspectedDataObjectDO(dataObject);
            }
            else
            if (keyPropertyInfo != null && valPropertyInfo != null)
            {
                object key = keyPropertyInfo.GetValue(obj);
                if (key is IDataObject keyDO)
                {
                    this.inspectedKeyDO = DataObjectInspectorMasterRepository.GetInspectedDataObjectDO(keyDO);
                }

                object val = valPropertyInfo.GetValue(obj);
                if (val is IDataObject valDO)
                {
                    this.inspectedValDO = DataObjectInspectorMasterRepository.GetInspectedDataObjectDO(valDO);
                }
            }

            this.inspectKeyButton.gameObject.SetActive(inspectedKeyDO != null);
            this.inspectValueButton.gameObject.SetActive(inspectedValDO != null);

            string valStr = (obj == null) ? "<NULL>" : obj.ToString();
            string displayText = $"{index}: {valStr}";
            this.text.text = displayText;
            return displayText;
        }


//*====================
//* PRIVATE
//*====================
        private void OnInspectButtonClicked()
        {
            this.inspectedValDO.isInspected.Value = true;
        }
    }
}