﻿using System;
using CoreDev.DataObjectInspector;
using CoreDev.Observable;
using UnityEngine;


namespace CoreDev.Framework
{
    public abstract class MonoBehaviourDO : MonoBehaviour, IName, IMonoBehaviour, IDataObject
    {
        protected event Action<MonoBehaviourDO> destroying = delegate { };

        [Bookmark]
        private OString transformName;
        public OString Name => transformName;
        private OBool isActive;
        public OBool IsActive => isActive;


//*====================
//* UNITY
//*====================
        protected virtual void Awake()
        {
            this.transformName = new OString(this.transform.name, this);
            this.isActive = new OBool(this.gameObject.activeSelf, this);
            
            this.transformName.RegisterForChanges(OnTransformNameChanged, false);
            this.isActive.RegisterForChanges(OnIsActiveChanged, false);
        }

        protected virtual void Start()
        {
            this.BindAspect(this);
            DataObjectMasterRepository.RegisterDataObject(this);
        }

        protected virtual void OnDestroy()
        {
            this.transformName?.UnregisterFromChanges(OnTransformNameChanged);
            this.isActive?.UnregisterFromChanges(OnIsActiveChanged);

            DataObjectMasterRepository.DestroyDataObject(this);
            this.UnbindAspect(this);
            this.FireDestroyingEvent();
        }

        protected void FireDestroyingEvent()
        {
            this.destroying(this);
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
        public virtual void RegisterForDestruction(Action<MonoBehaviourDO> callback)
        {
            destroying -= callback;
            destroying += callback;
        }

        public virtual void UnregisterFromDestruction(Action<MonoBehaviourDO> callback)
        {
            destroying -= callback;
        }

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