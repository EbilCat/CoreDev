using System;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;


namespace CoreDev.DataObjectInspector
{
    public class DataObjectInspectorDO : MonoBehaviour, IDataObject
    {
        private static DataObjectInspectorDO singleton;

        public OBool isOn = new OBool(false);
        public OString dataObjectFilterString = new OString(string.Empty);


        private void Awake()
        {
            if (singleton != null)
            {
                Destroy(this.gameObject);
                return;
            }

            singleton = this;
            this.isOn.DataObject = this;
            this.dataObjectFilterString.DataObject = this;

            this.BindAspect(this);
            DontDestroyOnLoad(this.gameObject);
        }

        private void OnDestroy()
        {
            this.UnbindAspect(this);
            if(singleton = this)
            {
                singleton = null;
            }
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