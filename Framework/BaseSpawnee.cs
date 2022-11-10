using System;
using CoreDev.Observable;
using CoreDev.Sequencing;
using UnityEngine;

namespace CoreDev.Framework
{
    public abstract class BaseSpawnee : MonoBehaviour, ISpawnee
    {
        private Action<object[]> startDependencyCheck;
        [SerializeField] private int fulfillmentAttemptLimit = 10;
        private int fulfillmentAttemptCount = 0;

        protected OBool initComplete = new OBool(false);
        public bool InitComplete => initComplete.Value;


//*====================
//* UNITY
//*====================
        protected virtual void Awake()
        {
            this.BeginInit();
        }

        protected virtual void OnDestroy()
        {
            UniversalTimer.UnscheduleCallback(startDependencyCheck);
            if (initComplete.Value)
            {
                this.ClearDependencies();
            }
        }


//*====================
//* BINDING
//*====================
        protected void BeginInit(object obj = null)
        {
            if (FulfillDependencies())
            {
                this.RegisterCallbacks();
                this.initComplete.Value = true;
            }
            else
            {
                if (startDependencyCheck == null) { startDependencyCheck = BeginInit; }
                UniversalTimer.ScheduleCallbackUnscaled(startDependencyCheck);
            }
        }


//*====================
//* VIRTUALS
//*====================
        public virtual void BindDO(IDataObject dataObject) { }

        public virtual void UnbindDO(IDataObject dataObject) { }

        protected virtual bool FulfillDependencies()
        {
            fulfillmentAttemptCount++;
            if (fulfillmentAttemptCount >= fulfillmentAttemptLimit)
            {
                Debug.LogWarning($"{this.name} ({this.GetType().Name}) failed to initialize after {fulfillmentAttemptCount} tries", this.gameObject);
            }

            bool fufilledDependencies = true;
            return fufilledDependencies;
        }

        protected virtual void RegisterCallbacks() { }

        protected virtual void UnregisterCallbacks() { }

        protected virtual void ClearDependencies(object obj = null)
        {
            this.initComplete.Value = false;
            fulfillmentAttemptCount = 0;
            this.UnregisterCallbacks();
        }


//*====================
//* PRIVATE
//*====================
        protected bool AttemptDependencyBind<T>(IDataObject dataObject, ref T target) where T : class, IDataObject
        {
            if (target == null && dataObject is T destinationTypeDO && dataObject != null)
            {
                target = destinationTypeDO;
                if (target != null)
                {
                    target.disposing -= ClearDependencies;
                    target.disposing += ClearDependencies;
                }
                return true;
            }
            return false;
        }

        protected void UnbindDependency<T>(IDataObject dataObject, ref T target) where T : class, IDataObject
        {
            if (dataObject is T && target == dataObject as T)
            {
                this.ClearDependencies();
            }
        }

        protected bool RetrieveDependency<T>(ref T target, Predicate<T> predicate = null) where T : class, IDataObject
        {
            if (target == null)
            {
                target = DataObjectMasterRepository.GetDataObject(predicate);
                if (target != null)
                {
                    target.disposing -= ClearDependencies;
                    target.disposing += ClearDependencies;
                }
            }

            bool fulfilled = target != null;
            return fulfilled;
        }

        protected void ClearDependency<T>(ref T target) where T : class, IDataObject
        {
            if (target != null)
            {
                target.disposing -= ClearDependencies;
                target = null;
            }
        }
    }
}