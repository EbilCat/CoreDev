using System;
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

        public OAction dataObjectFilterSubmitted = new OAction();
        public OAction observableVarFilterSubmitted = new OAction();


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
        public event Action<IDataObject> disposing;
        public void Dispose()
        {
            disposing?.Invoke(this);
        }
    }
}