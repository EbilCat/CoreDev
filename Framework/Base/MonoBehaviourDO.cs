using System;
using CoreDev.Observable;
using UnityEngine;


namespace CoreDev.Framework
{
    public abstract class MonoBehaviourDO : MonoBehaviour, ITransform
    {
        protected event Action<MonoBehaviourDO> destroying = delegate { };
        public Vector3 Pos_World => this.transform.position;
        public Quaternion Rot_World => this.transform.rotation;
        public Vector3 Scale_World => this.transform.lossyScale;

        private OString transformName;
        public OString Name => transformName;

        private OVector3 pos_Local;
        public OVector3 Pos_Local => pos_Local;

        private OQuaternion rot_Local;
        public OQuaternion Rot_Local => rot_Local;

        private OVector3 scale_Local;
        public OVector3 Scale_Local => scale_Local;


//*====================
//* UNITY
//*====================
        protected virtual void Awake()
        {
            this.transformName = new OString(this.transform.name, this);
            this.pos_Local = new OVector3(this.transform.localPosition, this);
            this.rot_Local = new OQuaternion(this.transform.localRotation, this);
            this.scale_Local = new OVector3(this.transform.localScale, this);

            this.transformName.RegisterForChanges(OnTransformNameChanged);
            this.pos_Local.RegisterForChanges(OnPos_LocalChanged);
            this.rot_Local.RegisterForChanges(OnRot_LocalChanged);
            this.scale_Local.RegisterForChanges(OnScale_LocalChanged);

        }

        protected virtual void Start()
        {
            this.BindAspect(this);
            DataObjectMasterRepository.RegisterDataObject(this);
        }

        protected virtual void OnDestroy()
        {
            this.transformName.UnregisterFromChanges(OnTransformNameChanged);
            this.pos_Local.UnregisterFromChanges(OnPos_LocalChanged);
            this.rot_Local.UnregisterFromChanges(OnRot_LocalChanged);
            this.scale_Local.UnregisterFromChanges(OnScale_LocalChanged);

            DataObjectMasterRepository.DestroyDataObject(this);
            this.UnbindAspect(this);
            this.FireDestroyingEvent();
        }

        protected void FireDestroyingEvent()
        {
            this.destroying(this);
        }


//*====================
//* CALLBACKS - ITransform
//*====================
        protected virtual void OnTransformNameChanged(ObservableVar<string> obj)
        {
            this.transform.name = obj.Value;
        }

        protected virtual void OnPos_LocalChanged(ObservableVar<Vector3> obj)
        {
            this.transform.localPosition = obj.Value;
        }

        protected virtual void OnRot_LocalChanged(ObservableVar<Quaternion> obj)
        {
            this.transform.localRotation = obj.Value;
        }

        protected virtual void OnScale_LocalChanged(ObservableVar<Vector3> obj)
        {
            this.transform.localScale = obj.Value;
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


    public interface ITransform : IDataObject
    {
        OString Name { get; }
        OVector3 Pos_Local { get; }
        OQuaternion Rot_Local { get; }
        OVector3 Scale_Local { get; }
        Vector3 Pos_World { get; }
        Quaternion Rot_World { get; }
        Vector3 Scale_World { get; }

        Vector3 WorldToLocalSpace(Vector3 pos_World);
        Vector3 LocalToWorld(Vector3 pos_Local);
        Quaternion WorldToLocal(Quaternion rot_World);
        Quaternion LocalToWorld(Quaternion rot_Local);
    }
}