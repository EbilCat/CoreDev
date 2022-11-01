using CoreDev.Framework;
using UnityEngine.UI;

namespace CoreDev.DataObjectInspector
{
    public class InspectedDataObjectCardHeaderText : BaseSpawnee
    {
        private InspectedDataObjectDO inspectedDataObjectDO;
        private Text text;


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
            fulfilled &= this.text = this.GetComponent<Text>();
            return fulfilled;
        }

        protected override void ClearDependencies(object obj = null)
        {
            base.ClearDependencies(obj);
            this.ClearDependency(ref inspectedDataObjectDO);
            this.text = null;
        }

        protected override void RegisterCallbacks()
        {
            this.text.text = this.inspectedDataObjectDO.DataObjectInstance.GetType().Name;
        }

        protected override void UnregisterCallbacks()
        {
            this.text.text = string.Empty;
        }
    }
}
