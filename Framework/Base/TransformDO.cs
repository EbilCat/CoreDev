﻿using System;
using CoreDev.Observable;
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
                this.transform.position = value;
                this.pos_Local.Value = this.transform.localPosition;
            }
        }

        public Quaternion Rot_World
        {
            get => this.transform.rotation;
            set
            {
                this.transform.rotation = value;
                this.rot_Local.Value = this.transform.localRotation;
            }
        }

        public Vector3 Scale_World => this.transform.lossyScale;

        private OString transformName;
        public OString Name => transformName;

        private OTransformPos pos_Local;
        public OVector3 Pos_Local => pos_Local;

        private OTransformRotation rot_Local;
        public OQuaternion Rot_Local => rot_Local;

        private OTransformScale scale_Local;
        public OVector3 Scale_Local => scale_Local;

        public Vector3 Forward => this.transform.forward;

        public Vector3 Right => this.transform.right;


//*====================
//* UNITY
//*====================
        protected virtual void Awake()
        {
            this.transformName = new OString(this.transform.name, this);
            this.pos_Local = new OTransformPos(this.transform.localPosition, this);
            this.rot_Local = new OTransformRotation(this.transform.localRotation, this);
            this.scale_Local = new OTransformScale(this.transform.localScale, this);

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
        public Vector3 LocalToWorld(Vector3 pos_Local)
        {
            return this.transform.TransformPoint(pos_Local);
        }

        public Vector3 LocalToWorldUnscaled(Vector3 pos_Local)
        {
            return this.transform.TransformPointUnscaled(pos_Local);
        }

        public Vector3 WorldToLocal(Vector3 pos_World)
        {
            return this.transform.InverseTransformPoint(pos_World);
        }

        public Vector3 WorldToLocalUnscaled(Vector3 pos_World)
        {
            return this.transform.InverseTransformPointUnscaled(pos_World);
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

        Vector3 WorldToLocal(Vector3 pos_World);
        Vector3 WorldToLocalUnscaled(Vector3 pos_World);
        Vector3 LocalToWorld(Vector3 pos_Local);
        Vector3 LocalToWorldUnscaled(Vector3 pos_Local);
        Quaternion WorldToLocal(Quaternion rot_World);
        Quaternion LocalToWorld(Quaternion rot_Local);

        Vector3 Forward { get; }
        Vector3 Right { get; }
    }
}