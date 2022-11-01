using CoreDev.Framework;
using UnityEngine.UI;

namespace CoreDev.DataObjectInspector
{
    public class InspectedDataObjectCardCloseButton : BaseSpawnee
    {
        private InspectedDataObjectDO inspectedDataObjectDO;
        private Button button;


//*====================
//* BINDING
//*====================
        public override void BindDO(IDataObject dataObject)
        {
            base.BindDO(dataObject);
            this.AttemptDependencyBind(dataObject, ref inspectedDataObjectDO);
        }
    
        public override void UnbindDO(IDataObject dataObject)
        {
            base.UnbindDO(dataObject);
            this.UnbindDependency(dataObject, ref inspectedDataObjectDO);
        }
    
        protected override bool FulfillDependencies()
        {
            bool fulfilled = base.FulfillDependencies();
            fulfilled &= inspectedDataObjectDO != null;
            fulfilled &= this.button = this.GetComponent<Button>();
            return fulfilled;
        }
    
        protected override void ClearDependencies(object obj = null)
        {
            base.ClearDependencies(obj);
            this.ClearDependency(ref inspectedDataObjectDO);
            this.button = null;
        }
    
        protected override void RegisterCallbacks()
        {
            this.button.onClick.AddListener(OnButtonClicked);
        }
    
        protected override void UnregisterCallbacks()
        {
            this.button.onClick.RemoveListener(OnButtonClicked);
        }


//*====================
//* CALLBACKS
//*====================
        private void OnButtonClicked()
        {
            this.inspectedDataObjectDO.isInspected.Value = false;
        }
    }
}