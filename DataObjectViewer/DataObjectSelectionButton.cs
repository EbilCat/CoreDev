using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;
using UnityEngine.UI;

namespace CoreDev.DataObjectInspector
{
    public class DataObjectSelectionButton : MonoBehaviour, ISpawnee
    {
        private InspectedDataObjectDO inspectedDataObjectDO;
        private Button button;
        private Text buttonText;
        private Image image;
        public string ButtonText { get { return buttonText.text; } }


//*====================
//* BINDING
//*====================
        public void BindDO(IDataObject dataObject)
        {
            if (inspectedDataObjectDO == null)
            {
                this.button = this.GetComponent<Button>();
                this.image = this.GetComponent<Image>();
                this.buttonText = this.GetComponentInChildren<Text>();

                this.button.onClick.AddListener(OnButtonClicked);

                this.inspectedDataObjectDO = dataObject as InspectedDataObjectDO;
                this.inspectedDataObjectDO.name.RegisterForChanges(OnNameChanged);
                this.inspectedDataObjectDO.matchesFilter.RegisterForChanges(OnMatchesFilterChanged);
                this.inspectedDataObjectDO.isInspected.RegisterForChanges(OnIsInspectedChanged);

            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedDataObjectDO && this.inspectedDataObjectDO == dataObject)
            {
                this.inspectedDataObjectDO?.name.UnregisterFromChanges(OnNameChanged);
                this.inspectedDataObjectDO?.matchesFilter.UnregisterFromChanges(OnMatchesFilterChanged);
                this.inspectedDataObjectDO?.isInspected.UnregisterFromChanges(OnIsInspectedChanged);
                this.inspectedDataObjectDO = null;

                this.button?.onClick.RemoveListener(OnButtonClicked);

                this.button = null;
                this.image = null;
                this.buttonText = null;
            }
        }


//*====================
//* CALLBACKS - InspectedDataObjectDO
//*====================
        private void OnNameChanged(ObservableVar<string> oName)
        {
            this.buttonText.text = oName.Value;
        }

        private void OnMatchesFilterChanged(ObservableVar<bool> oMatchesFilter)
        {
            bool matchesFilter = oMatchesFilter.Value;
            this.gameObject.SetActive(matchesFilter);
        }

        private void OnIsInspectedChanged(ObservableVar<bool> oIsInspected)
        {
            this.image.color = (oIsInspected.Value) ? Color.gray : Color.white;
        }


//*====================
//* CALLBACKS - Button
//*====================
        private void OnButtonClicked()
        {
            bool isInspected = this.inspectedDataObjectDO.isInspected.Value;
            this.inspectedDataObjectDO.isInspected.Value = !isInspected;
        }


//*====================
//* PRIVATE
//*====================
        // private string GetIdentifier(object dataObject, Type objType, BindingFlags bindingFlags, string identifierStr = "")
        // {
        //     FieldInfo[] fields = objType.GetFields(bindingFlags);

        //     for (int i = 0; i < fields.Length; i++)
        //     {
        //         FieldInfo fieldInfo = fields[i];
        //         Identifier identifier = fieldInfo.GetCustomAttribute<Identifier>(true);
        //         if (identifier != null)
        //         {
        //             object val = fieldInfo.GetValue(dataObject);
        //             Type valType = val.GetType();
        //             identifierStr += $"\r\n{fieldInfo.Name}:{valType.GetProperty("Value").GetValue(val)} ";
        //         }
        //     }

        //     return identifierStr;
        // }
    }
}