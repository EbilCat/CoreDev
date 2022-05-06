using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;


namespace CoreDev.DataObjectInspector
{
    public class DataObjectInspectorDO : MonoBehaviour, IDataObject
    {
        public OBool isOn = new OBool(false);
        public OString dataObjectFilterString = new OString(string.Empty);
        public OString observableVarFilterString = new OString(string.Empty);


        private void Awake()
        {
            this.isOn.DataObject = this;
            this.dataObjectFilterString.DataObject = this;
            this.observableVarFilterString.DataObject = this;

            this.BindAspect(this);
        }

        private void OnDestroy()
        {
            this.UnbindAspect(this);
        }


        //*====================
        //* IDataObject
        //*====================
        public void Dispose() { }
    }
}