using System;
using CoreDev.Observable;
using CoreDev.Sequencing;
using UnityEngine;

namespace CoreDev.Framework
{
    public abstract class BaseSpawnee : MonoBehaviour, ISpawnee
    {
        private Action<object[]> FuncBeginInit;
        private Action<object[]> FuncRegisterCallbacks;
        [SerializeField] private int fulfillmentAttemptLimit = 10;
        [SerializeField] private bool suppressFulfillmentFailedWarning = false;
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
            UniversalTimer.UnscheduleCallback(RegisterCallbacks);
            UniversalTimer.UnscheduleCallback(FuncBeginInit);
            if (initComplete.Value)
            {
                this.Dispose();
            }
        }


//*====================
//* BINDING
//*====================
        protected void BeginInit(object obj = null)
        {
            if (FulfillDependencies())
            {
                if (FuncRegisterCallbacks == null) { FuncRegisterCallbacks = RegisterCallbacks; }
                UniversalTimer.ScheduleCallbackFrames(RegisterCallbacks); //Delay by a frame to allow binding on child objects to complete before registration
            }
            else
            {
                if (FuncBeginInit == null) { FuncBeginInit = BeginInit; }
                UniversalTimer.ScheduleCallbackFrames(FuncBeginInit);
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
            if (fulfillmentAttemptCount >= fulfillmentAttemptLimit && suppressFulfillmentFailedWarning == false)
            {
                Debug.LogWarning($"{this.name} ({this.GetType().Name}) failed to initialize after {fulfillmentAttemptCount} tries", this.gameObject);
            }

            bool fufilledDependencies = true;
            return fufilledDependencies;
        }

        private void RegisterCallbacks(object[] obj)
        {
            this.RegisterCallbacks();
            this.initComplete.Value = true;
        }

        protected virtual void RegisterCallbacks() { }


        protected virtual void Dispose(object obj = null)
        { 
            this.UnregisterCallbacks();
            this.initComplete.Value = false;
            fulfillmentAttemptCount = 0;
            UniversalTimer.ScheduleCallbackFrames(ClearDependencies);
        }

        protected virtual void UnregisterCallbacks() { }

        protected virtual void ClearDependencies(object obj = null) { }


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
                    target.disposing -= Dispose;
                    target.disposing += Dispose;
                }
                return true;
            }
            return false;
        }

        protected void UnbindDependency<T>(IDataObject dataObject, ref T target) where T : class, IDataObject
        {
            if (dataObject is T && target == dataObject as T)
            {
                this.Dispose();
                // target = null;
            }
        }

        protected bool RetrieveDependency<T>(ref T target, Predicate<T> predicate = null) where T : class, IDataObject
        {
            if (target == null)
            {
                target = DataObjectMasterRepository.GetDataObject(predicate);
                if (target != null)
                {
                    target.disposing -= Dispose;
                    target.disposing += Dispose;
                }
            }

            bool fulfilled = target != null;
            return fulfilled;
        }

        protected void ClearDependency<T>(ref T target) where T : class, IDataObject
        {
            if (target != null)
            {
                target.disposing -= Dispose;
                target = null;
            }
        }
    }
}