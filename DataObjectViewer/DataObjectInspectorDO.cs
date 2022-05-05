using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;


namespace CoreDev.DataObjectInspector
{
    public class DataObjectInspectorDO : MonoBehaviour, IDataObject
    {
        public OBool isOn = new OBool(false);
        public OString filterString = new OString(string.Empty);


        private void Awake()
        {
            this.isOn.DataObject = this;
            this.filterString.DataObject = this;

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