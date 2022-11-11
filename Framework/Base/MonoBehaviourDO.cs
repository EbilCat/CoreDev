using System;
using CoreDev.DataObjectInspector;
using CoreDev.Observable;
using CoreDev.Sequencing;
using UnityEngine;


namespace CoreDev.Framework
{
    public abstract class MonoBehaviourDO : MonoBehaviour, IName, IMonoBehaviour, IDataObject
    {
        [Bookmark]
        private OString transformName;
        public OString Name => transformName;
        private OBool isActive;
        public OBool IsActive => isActive;

        public event Action<IDataObject> disposing;


//*====================
//* UNITY
//*====================
        protected virtual void Awake()
        {
            this.transformName = new OString(this.transform.name, this);
            this.isActive = new OBool(this.gameObject.activeSelf, this);

            this.transformName.RegisterForChanges(OnTransformNameChanged, false);
            this.isActive.RegisterForChanges(OnIsActiveChanged, false);

            UniversalTimer.ScheduleCallback(BindAndRegister);
        }

        protected virtual void BindAndRegister(object obj)
        {
            this.BindAspect(this);
            DataObjectMasterRepository.RegisterDataObject(this, false);
        }

        protected virtual void OnDestroy()
        {
            this.transformName?.UnregisterFromChanges(OnTransformNameChanged);
            this.isActive?.UnregisterFromChanges(OnIsActiveChanged);

            this.disposing?.Invoke(this);
            this.UnbindAspect(this);
        }


//*====================
//* CALLBACKS
//*====================
        protected virtual void OnTransformNameChanged(ObservableVar<string> obj)
        {
            this.transform.name = obj.Value;
        }

        private void OnIsActiveChanged(ObservableVar<bool> obj)
        {
            this.gameObject.SetActive(obj.Value);
        }


//*====================
//* MonoBehaviourDataObject
//*====================
        public virtual void Dispose()
        {
            Destroy(this.gameObject);
        }
    }

//*====================
//* INTERFACES
//*====================
    public interface IName : IDataObject
    {
        OString Name { get; }
    }

    public interface IMonoBehaviour : IName, IDataObject
    {
        OBool IsActive { get; }
    }
}