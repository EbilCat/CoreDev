using System;
using UnityEngine;


namespace CoreDev.Framework
{
    public abstract class MonoBehaviourDO : MonoBehaviour, IDataObject
    {
        protected event Action<MonoBehaviourDO> destroying = delegate { };
        public Vector3 Pos_World => this.transform.position;
        public Quaternion Rot_World => this.transform.rotation;
        public Vector3 Scale_World => this.transform.lossyScale;


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
            this.FireDestroyingEvent();
        }

        protected void FireDestroyingEvent()
        {
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


//*====================
//* UTILS
//*====================
        public Vector3 LocalToWorld(Vector3 pos_Local)
        {
            return this.transform.TransformPoint(pos_Local);
        }

        public Vector3 WorldToLocalSpace(Vector3 pos_World)
        {
            return this.transform.InverseTransformPoint(pos_World);
        }

        public Quaternion LocalToWorld(Quaternion rot_Local)
        {
            return this.transform.rotation * rot_Local;
        }

        public Quaternion WorldToLocal(Quaternion rot_World)
        {
            return Quaternion.Inverse(this.transform.rotation) * rot_World;
        }
    }
}