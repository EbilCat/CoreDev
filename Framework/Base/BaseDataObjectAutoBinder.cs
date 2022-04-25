using CoreDev.Framework;
using UnityEngine;


namespace CoreDev.Framework
{
    public abstract class BaseDataObjectAutoBinder<T> : MonoBehaviour
    {
//*====================
//* UNITY
//*====================
        protected virtual void Awake()
        {
            DataObjectMasterRepository.RegisterForCreation(OnDataObjectCreated);
            DataObjectMasterRepository.RegisterForDisposing(OnDataObjectDisposed);
        }

        protected virtual void OnDestroy()
        {
            DataObjectMasterRepository.UnregisterFromCreation(OnDataObjectCreated);
            DataObjectMasterRepository.UnregisterFromDisposing(OnDataObjectDisposed);
        }


//*====================
//* CALLBACKS - DataObjectMasterRepository
//*====================
        protected virtual void OnDataObjectCreated(IDataObject obj)
        {
            if (obj is T)
            {
                obj.BindAspect(this);
            }
        }

        protected virtual void OnDataObjectDisposed(IDataObject obj)
        {
            if (obj is T)
            {
                obj.UnbindAspect(this);
            }
        }
    }
}