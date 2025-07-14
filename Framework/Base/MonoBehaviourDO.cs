using System;
using System.Collections.Generic;
using CoreDev.DataObjectInspector;
using CoreDev.Observable;
using UnityEngine;


namespace CoreDev.Framework
{
    public abstract class MonoBehaviourDO : MonoBehaviour, IName, IMonoBehaviour, IDataObject
    {
        [Bookmark]
        private OString transformName;
        public OString Name => transformName;
        [Bookmark] private OBool isActive;
        public OBool IsActive => isActive;

        [SerializeField] private List<Component> aspects;

        public event Action<IDataObject> disposing;


//*====================
//* UNITY
//*====================
        private void Awake()
        {
            this.Init();
            this.BindAndRegister();
        }

        protected virtual void Init()
        {
            this.transformName = new OString(this.transform.name, this);
            this.isActive = new OBool(this.gameObject.activeSelf, this);
            this.transformName.RegisterForChanges(OnTransformNameChanged, false);
            this.isActive.RegisterForChanges(OnIsActiveChanged, false);
        }

        protected virtual void BindAndRegister()
        {
            this.BindAspect(this);
            for (int i = 0; i < aspects.Count; i++)
            {
                this.BindAspect(aspects[i]);
            }
            DataObjectMasterRepository.RegisterDataObject(this, false);
        }

        protected virtual void OnDestroy()
        {
            this.transformName?.UnregisterFromChanges(OnTransformNameChanged);
            this.isActive?.UnregisterFromChanges(OnIsActiveChanged);
            
            this.disposing?.Invoke(this);
            this.UnbindAspect(this);
            for (int i = 0; i < aspects.Count; i++)
            {
                this.UnbindAspect(aspects[i]);
            }
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
            if (this != null)
            {
                Destroy(this.gameObject);
            }
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