using System;
using CoreDev.Sequencing;
using UnityEngine;

namespace CoreDev.Framework
{
    public interface ISpawnee
    {
        void BindDO(IDataObject dataObject);

        void UnbindDO(IDataObject dataObject);
    }


    public abstract class BaseSpawnee<T> : MonoBehaviour, ISpawnee where T : class, IDataObject
    {
        protected T primeDO;
        private Action<object[]> AttemptBindingAction;
    

//*====================
//* UNITY
//*====================
        protected virtual void OnDestroy()
        {
            this.UnbindDO(this.primeDO);
            UniversalTimer.UnscheduleCallback(AttemptBindingAction);
        }
    

//*====================
//* BINDING
//*====================
        public virtual void BindDO(IDataObject dataObject)
        {
            if(dataObject is T && this.primeDO == null)
            {
                this.primeDO = dataObject as T;
                ProceedToBinding();
            }
        }

        protected void ProceedToBinding()
        {
            this.AttemptBindingAction = AttemptBinding;
            this.AttemptBindingAction.Invoke(null);
        }

        private void AttemptBinding(object obj = null)
        {
            if (FulfillDependencies())
            {
                this.RegisterCallbacks();
            }
            else
            {
                UniversalTimer.ScheduleCallback(AttemptBindingAction, 0.1f);
            }
        }

        public virtual void UnbindDO(IDataObject dataObject)
        {
            if(dataObject is T && this.primeDO == dataObject as T)
            {
                this.UnregisterCallbacks();
                this.ClearDependencies();
            }
        }


//*====================
//* ABSTRACT
//*====================
        protected virtual bool FulfillDependencies()
        {
            bool fufilledDependencies = true;
            return fufilledDependencies;
        }

        protected abstract void RegisterCallbacks();

        protected abstract void UnregisterCallbacks();

        protected virtual void ClearDependencies()
        {
            this.primeDO = null;
        }
    }
}