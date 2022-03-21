﻿using System;
using UnityEngine;


namespace CoreDev.Framework
{
    public abstract class MonoBehaviourDO : MonoBehaviour, IDataObject
    {
        private event Action<MonoBehaviourDO> destroying = delegate { };


//*====================
//* UNITY
//*====================
        protected virtual void Awake()
        {
            this.BindAspect(this);
            DataObjectMasterRepository.RegisterDataObject(this);
        }

        protected virtual void OnDestroy()
        {
            DataObjectMasterRepository.DestroyDataObject(this);
            this.UnbindAspect(this);
            this.destroying(this);
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
}