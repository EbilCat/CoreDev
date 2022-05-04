﻿using CoreDev.Observable;
using UnityEngine;


namespace CoreDev.Framework
{
    public abstract class TransformDO : MonoBehaviourDO, ITransform
    {
        public Vector3 Pos_World
        {
            get => this.transform.position;
            set
            {
                Transform parentTransform = this.transform.parent;
                this.pos_Local.Value = (parentTransform == null) ? value : parentTransform.InverseTransformPoint(value);
            }
        }

        public Quaternion Rot_World
        {
            get => this.transform.rotation;
            set
            {
                Transform parentTransform = this.transform.parent;
                this.rot_Local.Value = (parentTransform == null) ? value : Quaternion.Inverse(parentTransform.rotation) * value;
            }
        }

        public Vector3 Scale_World => this.transform.lossyScale;

        private OString transformName;
        public OString Name => transformName;

        private OTransformVector3 pos_Local;
        public OVector3 Pos_Local => pos_Local;

        private OTransformQuaternion rot_Local;
        public OQuaternion Rot_Local => rot_Local;

        private OTransformVector3 scale_Local;
        public OVector3 Scale_Local => scale_Local;

        public Vector3 Forward_World => this.transform.forward;

        public Vector3 Right_World => this.transform.right;


//*====================
//* UNITY
//*====================
        protected virtual void Awake()
        {
            this.transformName = new OString(this.transform.name, this);
            this.pos_Local = new OTransformVector3(this.transform.localPosition, this);
            this.rot_Local = new OTransformQuaternion(this.transform.localRotation, this);
            this.scale_Local = new OTransformVector3(this.transform.localScale, this);

            this.transformName.RegisterForChanges(OnTransformNameChanged);
            this.pos_Local.RegisterForChanges(OnPos_LocalChanged);
            this.rot_Local.RegisterForChanges(OnRot_LocalChanged);
            this.scale_Local.RegisterForChanges(OnScale_LocalChanged);
        }

        protected override void OnDestroy()
        {
            this.transformName.UnregisterFromChanges(OnTransformNameChanged);
            this.pos_Local.UnregisterFromChanges(OnPos_LocalChanged);
            this.rot_Local.UnregisterFromChanges(OnRot_LocalChanged);
            this.scale_Local.UnregisterFromChanges(OnScale_LocalChanged);

            base.OnDestroy();
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
//* Transform Wrappers
//*====================
        public Vector3 WorldToParent(Vector3 pos_World)
        {
            Transform transformParent = this.transform.parent;
            if(transformParent == null)
            {
                return pos_World;
            }
            else
            {
                return transformParent.InverseTransformPoint(pos_World);
            }
        }

        public Vector3 ParentToWorld(Vector3 pos_Parent)
        {
            Transform transformParent = this.transform.parent;
            if(transformParent == null)
            {
                return pos_Parent;
            }
            else
            {
                return transformParent.TransformPoint(pos_Parent);
            }
        }

        public Vector3 ThisToWorld(Vector3 pos_This)
        {
            return this.transform.TransformPoint(pos_This);
        }

        public Vector3 WorldToThis(Vector3 pos_World)
        {
            return this.transform.InverseTransformPoint(pos_World);
        }

        public Quaternion ThisToWorld(Quaternion rot_This)
        {
            return this.transform.rotation * rot_This;
        }

        public Quaternion WorldToThis(Quaternion rot_World)
        {
            return Quaternion.Inverse(this.transform.rotation) * rot_World;
        }

        public Vector3 ThisToWorldDirection(Vector3 dir_This)
        {
            return this.transform.TransformDirection(dir_This);
        }

        public Vector3 WorldToThisDirection(Vector3 dir_World)
        {
            return this.transform.InverseTransformDirection(dir_World);
        }
    }


//*====================
//* INTERFACES
//*====================
    public interface IName : IDataObject
    {
        OString Name { get; }
    }

    public interface ITransform : IName, IDataObject
    {

        Vector3 Pos_World { get; set; }
        Quaternion Rot_World { get; set; }
        Vector3 Scale_World { get; }
        OVector3 Pos_Local { get; }
        OQuaternion Rot_Local { get; }
        OVector3 Scale_Local { get; }

        Vector3 WorldToThis(Vector3 pos_World);
        Vector3 ThisToWorld(Vector3 pos_Local);
        Vector3 WorldToParent(Vector3 pos_World);
        Vector3 ParentToWorld(Vector3 pos_Parent);
        Quaternion WorldToThis(Quaternion rot_World);
        Quaternion ThisToWorld(Quaternion rot_Local);
        Vector3 ThisToWorldDirection(Vector3 direction);
        Vector3 WorldToThisDirection(Vector3 direction);

        Vector3 Forward_World { get; }
        Vector3 Right_World { get; }
    }
}