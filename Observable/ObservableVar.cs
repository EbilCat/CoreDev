using System;
using System.Collections.Generic;
using CoreDev.Extensions;
using CoreDev.Framework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CoreDev.Observable
{
    public interface IObservableVar
    {
        IDataObject DataObject { get; set; }

        string ToString();
        string GetCallbacks();
        void GetCallbacks(List<string> callbacks);
        void SetValueFromString(string strVal);

        byte[] ToBytes();
        void SetValueFromBytes(byte[] bytes);

        void RegisterForChanges(Action<IObservableVar> callback, bool fireCallbackOnRegistration);
        void UnregisterFromChanges(Action<IObservableVar> callback);
    }


//*====================
//* OEvent
//*====================
    public class OEvent : ObservableVar<object>
    {
        public OEvent() : base(null) { }
        public OEvent(IDataObject dataObject) : base(null, dataObject) { }

        public override void RegisterForChanges(Action<IObservableVar> callback, bool fireCallbackOnRegistration = false)
        {
            base.RegisterForChanges(callback, fireCallbackOnRegistration);
        }

        public override void RegisterForChanges(Action<ObservableVar<object>> callback, bool fireCallbackOnRegistration = false)
        {
            base.RegisterForChanges(callback, fireCallbackOnRegistration);
        }

        public virtual void Fire(object obj = null)
        {
            this.FireCallbacks_Moderated(obj);
        }
        
        protected override bool AreEqual(object var, object value)
        {
            return false;
        }

        public override string ToString()
        {
            return string.Empty;
        }

        public override void SetValueFromString(string strVal)
        {
            this.Value = strVal;
        }
    }


//*====================
//* SByte
//*====================
    [Serializable]
    public class OSByte : ObservableVar<SByte>
    {
        public OSByte() : base(default(SByte)) { }
        public OSByte(IDataObject dataObject) : base(default(SByte), dataObject) { }
        public OSByte(SByte initValue) : base(initValue) { }
        public OSByte(SByte initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(SByte var, SByte value) { return (var == value); }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                this.Value = Convert.ToSByte(strVal);
            }
            catch
            {
                Debug.Log("Error converting ToSByte");
            }
        }
    }


//*====================
//* Byte
//*====================
    [Serializable]
    public class OByte : ObservableVar<byte>
    {
        public OByte() : base(default(byte)) { }
        public OByte(IDataObject dataObject) : base(default(byte), dataObject) { }
        public OByte(byte initValue) : base(initValue) { }
        public OByte(byte initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(byte var, byte value) { return (var == value); }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                this.Value = Convert.ToByte(strVal);
            }
            catch
            {
                Debug.Log("Error converting ToByte");
            }
        }
    }


//*====================
//* Int16
//*====================
    [Serializable]
    public class OInt16 : ObservableVar<Int16>
    {
        public OInt16() : base(default(Int16)) { }
        public OInt16(IDataObject dataObject) : base(default(Int16), dataObject) { }
        public OInt16(Int16 initValue) : base(initValue) { }
        public OInt16(Int16 initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(Int16 var, Int16 value) { return (var == value); }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                this.Value = Convert.ToInt16(strVal);
            }
            catch
            {
                Debug.Log("Error converting ToInt16");
            }
        }
    }

    [Serializable]
    public class OShort : ObservableVar<short>
    {
        public OShort() : base(default(short)) { }
        public OShort(IDataObject dataObject) : base(default(short), dataObject) { }
        public OShort(short initValue) : base(initValue) { }
        public OShort(short initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(short var, short value) { return (var == value); }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                this.Value = Convert.ToInt16(strVal);
            }
            catch
            {
                Debug.Log("Error converting ToInt16");
            }
        }
    }


//*====================
//* UInt16
//*====================
    [Serializable]
    public class OUInt16 : ObservableVar<UInt16>
    {
        public OUInt16() : base(default(UInt16)) { }
        public OUInt16(IDataObject dataObject) : base(default(UInt16), dataObject) { }
        public OUInt16(UInt16 initValue) : base(initValue) { }
        public OUInt16(UInt16 initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(UInt16 var, UInt16 value) { return (var == value); }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                this.Value = Convert.ToUInt16(strVal);
            }
            catch
            {
                Debug.Log("Error converting ToUInt16");
            }
        }
    }


//*====================
//* Int32
//*====================
    [Serializable]
    public class OInt : ObservableVar<int>
    {
        public OInt() : base(default(int)) { }
        public OInt(IDataObject dataObject) : base(default(int), dataObject) { }
        public OInt(int initValue) : base(initValue) { }
        public OInt(int initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(int var, int value) { return (var == value); }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                this.Value = Convert.ToInt32(strVal);
            }
            catch
            {
                Debug.Log("Error converting ToInt32");
            }
        }
    }


    [Serializable]
    public class OInt32 : ObservableVar<Int32>
    {
        public OInt32() : base(default(Int32)) { }
        public OInt32(IDataObject dataObject) : base(default(Int32), dataObject) { }
        public OInt32(Int32 initValue) : base(initValue) { }
        public OInt32(Int32 initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(Int32 var, Int32 value) { return (var == value); }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                this.Value = Convert.ToInt32(strVal);
            }
            catch
            {
                Debug.Log("Error converting ToInt32");
            }
        }
    }


//*====================
//* UInt32
//*====================
    [Serializable]
    public class OUInt : ObservableVar<uint>
    {
        public OUInt() : base(default(uint)) { }
        public OUInt(IDataObject dataObject) : base(default(uint), dataObject) { }
        public OUInt(uint initValue) : base(initValue) { }
        public OUInt(uint initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(uint var, uint value) { return (var == value); }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                this.Value = Convert.ToUInt32(strVal);
            }
            catch
            {
                Debug.Log("Error converting ToUInt32");
            }
        }
    }


//*====================
//* Int64
//*====================
    [Serializable]
    public class OInt64 : ObservableVar<Int64>
    {
        public OInt64() : base(default(Int64)) { }
        public OInt64(IDataObject dataObject) : base(default(Int64), dataObject) { }
        public OInt64(Int64 initValue) : base(initValue) { }
        public OInt64(Int64 initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(Int64 var, Int64 value) { return (var == value); }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                this.Value = Convert.ToInt64(strVal);
            }
            catch
            {
                Debug.Log("Error converting ToInt64");
            }
        }
    }


    [Serializable]
    public class OLong : ObservableVar<long>
    {
        public OLong() : base(default(long)) { }
        public OLong(IDataObject dataObject) : base(default(long), dataObject) { }
        public OLong(long initValue) : base(initValue) { }
        public OLong(long initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(long var, long value) { return (var == value); }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                this.Value = Convert.ToInt64(strVal);
            }
            catch
            {
                Debug.Log("Error converting ToInt64");
            }
        }
    }


//*====================
//* UInt64
//*====================
    [Serializable]
    public class OUInt64 : ObservableVar<UInt64>
    {
        public OUInt64() : base(default(UInt64)) { }
        public OUInt64(IDataObject dataObject) : base(default(UInt64), dataObject) { }
        public OUInt64(UInt64 initValue) : base(initValue) { }
        public OUInt64(UInt64 initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(UInt64 var, UInt64 value) { return (var == value); }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                this.Value = Convert.ToUInt64(strVal);
            }
            catch
            {
                Debug.Log("Error converting ToUInt64");
            }
        }
    }

    [Serializable]
    public class OULong : ObservableVar<ulong>
    {
        public OULong() : base(default(ulong)) { }
        public OULong(IDataObject dataObject) : base(default(ulong), dataObject) { }
        public OULong(ulong initValue) : base(initValue) { }
        public OULong(ulong initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(ulong var, ulong value) { return (var == value); }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                this.Value = Convert.ToUInt64(strVal);
            }
            catch
            {
                Debug.Log("Error converting ToUInt64");
            }
        }
    }


//*====================
//* Bool
//*====================
    [Serializable]
    public class OBool : ObservableVar<bool>
    {
        public OBool() : base(default(bool)) { }
        public OBool(IDataObject dataObject) : base(default(bool), dataObject) { }
        public OBool(bool initValue) : base(initValue) { }
        public OBool(bool initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(bool var, bool value) { return (var == value); }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                this.Value = Convert.ToBoolean(strVal);
            }
            catch
            {
                Debug.Log("Error converting ToBoolean");
            }
        }
    }


//*====================
//* Float
//*====================
    [Serializable]
    public class OFloat : ObservableVar<float>
    {
        public OFloat() : base(default(float)) { }
        public OFloat(IDataObject dataObject) : base(default(float), dataObject) { }
        public OFloat(float initValue) : base(initValue) { }
        public OFloat(float initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(float var, float value) { return var.AlmostEquals(value); }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                this.Value = float.Parse(strVal);
            }
            catch
            {
                Debug.Log("Error converting ToFloat");
            }
        }
    }


//*====================
//* Double
//*====================
    [Serializable]
    public class ODouble : ObservableVar<double>
    {
        public ODouble() : base(default(double)) { }
        public ODouble(IDataObject dataObject) : base(default(double), dataObject) { }
        public ODouble(double initValue) : base(initValue) { }
        public ODouble(double initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(double var, double value) { return var.IsEquals(value); }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                this.Value = double.Parse(strVal);
            }
            catch
            {
                Debug.Log("Error converting ToDouble");
            }
        }
    }


//*====================
//* String
//*====================
    [Serializable]
    public class OString : ObservableVar<string>
    {
        public OString() : base(default(string)) { }
        public OString(IDataObject dataObject) : base(default(string), dataObject) { }
        public OString(string initValue) : base(initValue) { }
        public OString(string initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(string var, string value) { return var == value; }

        public override void SetValueFromString(string strVal)
        {
            this.Value = strVal;
        }
    }


//*====================
//* Vector2
//*====================
    [Serializable]
    public class OVector2 : ObservableVar<Vector2>
    {
        public OVector2() : base(default(Vector2)) { }
        public OVector2(IDataObject dataObject) : base(default(Vector2), dataObject) { }
        public OVector2(Vector2 initValue) : base(initValue) { }
        public OVector2(Vector2 initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(Vector2 var, Vector2 value)
        {
            bool areEqual = true;
            areEqual = areEqual && var.x.AlmostEquals(value.x);
            areEqual = areEqual && var.y.AlmostEquals(value.y);
            return areEqual;
        }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                strVal = strVal.Replace("(", string.Empty);
                strVal = strVal.Replace(")", string.Empty);

                string[] splitArr = strVal.Split(',');
                float x = float.Parse(splitArr[0]);
                float y = float.Parse(splitArr[1]);
                this.Value = new Vector2(x, y);
            }
            catch
            {
                Debug.Log("Error converting to Vector2");
            }
        }
    }


//*====================
//* Vector3
//*====================
    [Serializable]
    public class OVector3 : ObservableVar<Vector3>
    {
        public OVector3() : base(default(Vector3)) { }
        public OVector3(IDataObject dataObject) : base(default(Vector3), dataObject) { }
        public OVector3(Vector3 initValue) : base(initValue) { }
        public OVector3(Vector3 initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(Vector3 var, Vector3 value)
        {
            bool areEqual = true;
            areEqual = areEqual && var.x.AlmostEquals(value.x);
            areEqual = areEqual && var.y.AlmostEquals(value.y);
            areEqual = areEqual && var.z.AlmostEquals(value.z);
            return areEqual;
        }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                strVal = strVal.Replace("(", string.Empty);
                strVal = strVal.Replace(")", string.Empty);

                string[] splitArr = strVal.Split(',');
                float x = float.Parse(splitArr[0]);
                float y = float.Parse(splitArr[1]);
                float z = float.Parse(splitArr[2]);
                this.Value = new Vector3(x, y, z);
            }
            catch
            {
                Debug.Log("Error converting to Vector3");
            }
        }
    }


//*====================
//* RaycastResult
//*====================
    [Serializable]
    public class ORaycastResult : ObservableVar<RaycastResult>
    {
        public ORaycastResult() : base(default(RaycastResult)) { }
        public ORaycastResult(IDataObject dataObject) : base(default(RaycastResult), dataObject) { }
        public ORaycastResult(RaycastResult initValue) : base(initValue) { }
        public ORaycastResult(RaycastResult initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(RaycastResult var, RaycastResult value)
        {
            return var.Equals(value);
        }
    }


//*====================
//* AudioClip
//*====================
    [Serializable]
    public class OAudioClip : ObservableVar<AudioClip>
    {
        public OAudioClip() : base(default(AudioClip)) { }
        public OAudioClip(IDataObject dataObject) : base(default(AudioClip), dataObject) { }
        public OAudioClip(AudioClip initValue) : base(initValue) { }
        public OAudioClip(AudioClip initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(AudioClip var, AudioClip value)
        {
            return var == value;
        }
    }


//*====================
//* LayerMask
//*====================
    [Serializable]
    public class OLayerMask : ObservableVar<LayerMask>
    {
        public OLayerMask() : base(default(LayerMask)) { }
        public OLayerMask(IDataObject dataObject) : base(default(LayerMask), dataObject) { }
        public OLayerMask(LayerMask initValue) : base(initValue) { }
        public OLayerMask(LayerMask initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(LayerMask var, LayerMask value)
        {
            bool areEqual = (var.value == value);
            return areEqual;
        }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                string[] splitArr = strVal.Split(',');
                int mask = LayerMask.GetMask(splitArr);
                this.Value = mask;
            }
            catch
            {
                Debug.Log("Error converting LayerMask");
            }
        }
    }


//*====================
//* Color
//*====================
    [Serializable]
    public class OColor : ObservableVar<Color>
    {
        public OColor() : base(default(Color)) { }
        public OColor(IDataObject dataObject) : base(default(Color), dataObject) { }
        public OColor(Color initValue) : base(initValue) { }
        public OColor(Color initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(Color var, Color value)
        {
            bool areEqual = true;
            areEqual = areEqual && var.r.AlmostEquals(value.r);
            areEqual = areEqual && var.g.AlmostEquals(value.g);
            areEqual = areEqual && var.b.AlmostEquals(value.b);
            areEqual = areEqual && var.a.AlmostEquals(value.a);
            return areEqual;
        }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                strVal = strVal.Replace("RGBA(", string.Empty);
                strVal = strVal.Replace(")", string.Empty);

                string[] splitArr = strVal.Split(',');
                float r = float.Parse(splitArr[0]);
                float g = float.Parse(splitArr[1]);
                float b = float.Parse(splitArr[2]);
                float a = float.Parse(splitArr[3]);
                this.Value = new Color(r, g, b, a);
            }
            catch
            {
                Debug.Log("Error converting to Color");
            }
        }
    }


//*====================
//* DateTime
//*====================
    [Serializable]
    public class ODateTime : ObservableVar<DateTime>
    {
        public ODateTime() : base(default(DateTime)) { }
        public ODateTime(IDataObject dataObject) : base(default(DateTime), dataObject) { }
        public ODateTime(DateTime initValue) : base(initValue) { }
        public ODateTime(DateTime initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(DateTime var, DateTime value) { return var.Equals(value); }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                this.Value = DateTime.Parse(strVal);
            }
            catch
            {
                Debug.Log("Error converting ToDateTime");
            }
        }
    }


//*====================
//* Texture
//*====================
    [Serializable]
    public class OTexture : ObservableVar<Texture>
    {
        public OTexture() : base(default(Texture)) { }
        public OTexture(IDataObject dataObject) : base(default(Texture), dataObject) { }
        public OTexture(Texture initValue) : base(initValue) { }
        public OTexture(Texture initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(Texture var, Texture value) { return var == value; }
    }


//*====================
//* Quaternion
//*====================
    [Serializable]
    public class OQuaternion : ObservableVar<Quaternion>
    {
        public OQuaternion() : base(default(Quaternion)) { }
        public OQuaternion(IDataObject dataObject) : base(default(Quaternion), dataObject) { }
        public OQuaternion(Quaternion initValue) : base(initValue) { }
        public OQuaternion(Quaternion initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(Quaternion var, Quaternion value)
        {
            bool areEqual = true;
            areEqual = areEqual && var.x.AlmostEquals(value.x, 8);
            areEqual = areEqual && var.y.AlmostEquals(value.y, 8);
            areEqual = areEqual && var.z.AlmostEquals(value.z, 8);
            areEqual = areEqual && var.w.AlmostEquals(value.w, 8);
            return areEqual;
        }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                strVal = strVal.Replace("(", string.Empty);
                strVal = strVal.Replace(")", string.Empty);

                string[] splitArr = strVal.Split(',');
                float x = float.Parse(splitArr[0]);
                float y = float.Parse(splitArr[1]);
                float z = float.Parse(splitArr[2]);
                float w = float.Parse(splitArr[3]);
                this.Value = new Quaternion(x, y, z, w);
            }
            catch
            {
                Debug.Log("Error converting to Quaternion");
            }
        }
    }


//*====================
//* Camera
//*====================
    [Serializable]
    public class OCamera : ObservableVar<Camera>
    {
        public OCamera() : base(default(Camera)) { }
        public OCamera(IDataObject dataObject) : base(default(Camera), dataObject) { }
        public OCamera(Camera initValue) : base(initValue) { }
        public OCamera(Camera initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(Camera var, Camera value) { return var == value; }
    }


//*====================
//* GameObject
//*====================
    [Serializable]
    public class OGameObject : ObservableVar<GameObject>
    {
        public OGameObject() : base(default(GameObject)) { }
        public OGameObject(IDataObject dataObject) : base(default(GameObject), dataObject) { }
        public OGameObject(GameObject initValue) : base(initValue) { }
        public OGameObject(GameObject initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(GameObject var, GameObject value) { return var == value; }
    }


//*====================
//* Transform
//*====================
    [Serializable]
    public class OTransform : ObservableVar<Transform>
    {
        public OTransform() : base(default(Transform)) { }
        public OTransform(IDataObject dataObject) : base(default(Transform), dataObject) { }
        public OTransform(Transform initValue) : base(initValue) { }
        public OTransform(Transform initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(Transform var, Transform value) { return var == value; }

        public override void SetValueFromString(string strVal)
        {
            GameObject go = GameObject.Find(strVal);
            this.Value = go?.transform;
        }
    }


//*====================
//* RectTransform
//*====================
    [Serializable]
    public class ORectTransform : ObservableVar<RectTransform>
    {
        public ORectTransform() : base(default(RectTransform)) { }
        public ORectTransform(IDataObject dataObject) : base(default(RectTransform), dataObject) { }
        public ORectTransform(RectTransform initValue) : base(initValue) { }
        public ORectTransform(RectTransform initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(RectTransform var, RectTransform value) { return var == value; }
    }


//*====================
//* Bounds
//*====================
    [Serializable]
    public class OBounds : ObservableVar<Bounds>
    {
        public OBounds() : base(default(Bounds)) { }
        public OBounds(IDataObject dataObject) : base(default(Bounds), dataObject) { }
        public OBounds(Bounds initValue) : base(initValue) { }
        public OBounds(Bounds initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(Bounds var, Bounds value) { return var.Equals(value); }
    }

//*====================
//* InputButton
//*====================
    [Serializable]
    public class OInputButton : ObservableVar<PointerEventData.InputButton>
    {
        public OInputButton() : base(default(PointerEventData.InputButton)) { }
        public OInputButton(IDataObject dataObject) : base(default(PointerEventData.InputButton), dataObject) { }
        public OInputButton(PointerEventData.InputButton initValue) : base(initValue) { }
        public OInputButton(PointerEventData.InputButton initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(PointerEventData.InputButton var, PointerEventData.InputButton value) { return (var == value); }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                object enumVal = Enum.Parse(typeof(PointerEventData.InputButton), strVal, true);
                this.Value = (PointerEventData.InputButton)enumVal;
            }
            catch
            {
                Debug.Log("Error converting ToInputButton");
            }
        }
    }


//*====================
//* RenderTextureFormat
//*====================
    [Serializable]
    public class ORenderTextureFormat : ObservableVar<RenderTextureFormat>
    {
        public ORenderTextureFormat() : base(default(RenderTextureFormat)) { }
        public ORenderTextureFormat(IDataObject dataObject) : base(default(RenderTextureFormat), dataObject) { }
        public ORenderTextureFormat(RenderTextureFormat initValue) : base(initValue) { }
        public ORenderTextureFormat(RenderTextureFormat initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(RenderTextureFormat var, RenderTextureFormat value) { return (var == value); }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                object enumVal = Enum.Parse(typeof(RenderTextureFormat), strVal, true);
                this.Value = (RenderTextureFormat)enumVal;
            }
            catch
            {
                Debug.Log("Error converting ToRenderTextureFormat");
            }
        }
    }


//*====================
//* FilterMode
//*====================
    [Serializable]
    public class OFilterMode : ObservableVar<FilterMode>
    {
        public OFilterMode() : base(default(FilterMode)) { }
        public OFilterMode(IDataObject dataObject) : base(default(FilterMode), dataObject) { }
        public OFilterMode(FilterMode initValue) : base(initValue) { }
        public OFilterMode(FilterMode initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(FilterMode var, FilterMode value) { return (var == value); }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                object enumVal = Enum.Parse(typeof(FilterMode), strVal, true);
                this.Value = (FilterMode)enumVal;
            }
            catch
            {
                Debug.Log("Error converting ToFilterMode");
            }
        }
    }


//*====================
//* RenderTexture
//*====================
    [Serializable]
    public class ORenderTexture : ObservableVar<RenderTexture>
    {
        public ORenderTexture() : base(default(RenderTexture)) { }
        public ORenderTexture(IDataObject dataObject) : base(default(RenderTexture), dataObject) { }
        public ORenderTexture(RenderTexture initValue) : base(initValue) { }
        public ORenderTexture(RenderTexture initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(RenderTexture var, RenderTexture value) { return var == value; }
    }


//*====================
//* TransformDO
//*====================
    [Serializable]
    public class OTransformDO : ObservableVar<TransformDO>
    {
        public OTransformDO() : base(default(TransformDO)) { }
        public OTransformDO(IDataObject dataObject) : base(default(TransformDO), dataObject) { }
        public OTransformDO(TransformDO initValue) : base(initValue) { }
        public OTransformDO(TransformDO initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(TransformDO var, TransformDO value)
        {
            return var == value;
        }
        public override void SetValueFromString(string strVal)
        {
            string strValTrimmed = strVal.Trim();
            if (strValTrimmed.Length == 0)
            {
                this.Value = null;
            }
            else
            {
                this.Value = DataObjectMasterRepository.GetDataObject<TransformDO>(x => x.Name.Value.Equals(strVal));
            }
        }
        public override string ToString()
        {
            TransformDO transformDO = this.Value;
            if (transformDO != null)
            {
                return transformDO.Name.Value;
            }
            return string.Empty;
        }
    }


//*====================
//* Type
//*====================
    [Serializable]
    public class OType : ObservableVar<Type>
    {
        public OType() : base(default(Type)) { }
        public OType(IDataObject dataObject) : base(default(Type), dataObject) { }
        public OType(Type initValue) : base(initValue) { }
        public OType(Type initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(Type var, Type value) { return var.Equals(value); }
    }


//*====================
//* Objects
//*====================
    [Serializable]
    public class OObject<TObject> : ObservableVar<TObject> where TObject : struct, IComparable, IConvertible
    {
        public OObject() : base(default(TObject)) { }
        public OObject(IDataObject dataObject) : base(default(TObject), dataObject) { }
        public OObject(TObject initValue) : base(initValue) { }
        public OObject(TObject initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(TObject var, TObject value)
        {
            bool areEquals = var.Equals(value);
            return areEquals;
        }
    }


//*====================
//* Enums
//*====================
    [Serializable]
    public class OEnum<TEnum> : ObservableVar<TEnum> where TEnum : struct, IComparable, IConvertible
    {
        public OEnum() : base(default(TEnum)) { }
        public OEnum(IDataObject dataObject) : base(default(TEnum), dataObject) { }
        public OEnum(TEnum initValue) : base(initValue) { }
        public OEnum(TEnum initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(TEnum var, TEnum value)
        {
            bool areEquals = var.Equals(value);
            return areEquals;
        }

        public override void SetValueFromString(string strVal)
        {
            try
            {
                object enumVal = Enum.Parse(typeof(TEnum), strVal, true);
                this.Value = (TEnum)enumVal;
            }
            catch
            {
                Debug.Log("Error converting ToEnum");
            }
        }
    }
}