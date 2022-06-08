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
                this.pos_Local.Value = this.WorldToParent(value);
            }
        }

        public Quaternion Rot_World
        {
            get => this.transform.rotation;
            set
            {
                this.rot_Local.Value = this.WorldToParent(value);
            }
        }

        public Vector3 Scale_World => this.transform.lossyScale;

        private OTransform transformParent;
        public OTransform TransformParent => transformParent;

        private OVector3 pos_Local;
        public OVector3 Pos_Local => pos_Local;

        private OQuaternion rot_Local;
        public OQuaternion Rot_Local => rot_Local;

        private OVector3 scale_Local;
        public OVector3 Scale_Local => scale_Local;

        public Vector3 Forward_World => this.transform.forward;

        public Vector3 Right_World => this.transform.right;


//*====================
//* UNITY
//*====================
        protected override void Awake()
        {
            base.Awake();
            this.transformParent = new OTransform(this.transform.parent, this);
            this.pos_Local = new OVector3(this.transform.localPosition, this);
            this.rot_Local = new OQuaternion(this.transform.localRotation, this);
            this.scale_Local = new OVector3(this.transform.localScale, this);

            this.transformParent.RegisterForChanges(OnParentChanged, false);
            this.pos_Local.RegisterForChanges(OnPos_LocalChanged, false);
            this.rot_Local.RegisterForChanges(OnRot_LocalChanged, false);
            this.scale_Local.RegisterForChanges(OnScale_LocalChanged, false);
        }

        protected override void OnDestroy()
        {
            this.transformParent.UnregisterFromChanges(OnParentChanged);
            this.pos_Local.UnregisterFromChanges(OnPos_LocalChanged);
            this.rot_Local.UnregisterFromChanges(OnRot_LocalChanged);
            this.scale_Local.UnregisterFromChanges(OnScale_LocalChanged);

            base.OnDestroy();
        }


//*====================
//* CALLBACKS - ITransform
//*====================
        private void OnParentChanged(ObservableVar<Transform> obj)
        {
            this.transform.SetParent(obj.Value);
            this.Pos_Local.Value = this.transform.localPosition;
            this.Rot_Local.Value = this.transform.localRotation;
            this.Scale_Local.Value = this.transform.localScale;
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


        public Vector3 WorldToParent(Vector3 pos_World)
        {
            Transform transformParent = this.transform.parent;
            Vector3 pos_Parent = pos_World;
            if (transformParent != null)
            {
                pos_Parent = transformParent.InverseTransformPoint(pos_World);
            }
            return pos_Parent;
        }

        public Vector3 ParentToWorld(Vector3 pos_Parent)
        {
            Transform transformParent = this.transform.parent;
            Vector3 pos_World = pos_Parent;
            if (transformParent != null)
            {
                pos_World = transformParent.TransformPoint(pos_Parent);
            }
            return pos_World;
        }

        public Quaternion WorldToParent(Quaternion rot_World)
        {
            Transform transformParent = this.transform.parent;
            Quaternion rot_Parent = rot_World;
            if (transformParent != null)
            {
                rot_Parent = Quaternion.Inverse(transformParent.rotation) * rot_World;
            }
            return rot_Parent;
        }

        public Quaternion ParentToWorld(Quaternion rot_Parent)
        {
            Transform transformParent = this.transform.parent;
            Quaternion rot_World = rot_Parent;
            if (transformParent != null)
            {
                rot_World = transformParent.rotation * rot_Parent;
            }
            return rot_World;
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
    public interface ITransform : IMonoBehaviour, IDataObject
    {
        Vector3 Pos_World { get; set; }
        Quaternion Rot_World { get; set; }
        Vector3 Scale_World { get; }
        OVector3 Pos_Local { get; }
        OQuaternion Rot_Local { get; }
        OVector3 Scale_Local { get; }

        Vector3 WorldToThis(Vector3 pos_World);
        Vector3 ThisToWorld(Vector3 pos_Local);
        Quaternion WorldToThis(Quaternion rot_World);
        Quaternion ThisToWorld(Quaternion rot_Local);

        Vector3 WorldToParent(Vector3 pos_World);
        Vector3 ParentToWorld(Vector3 pos_Parent);
        Quaternion WorldToParent(Quaternion rot_World);
        Quaternion ParentToWorld(Quaternion rot_Parent);

        Vector3 ThisToWorldDirection(Vector3 direction);
        Vector3 WorldToThisDirection(Vector3 direction);

        Vector3 Forward_World { get; }
        Vector3 Right_World { get; }
    }
}