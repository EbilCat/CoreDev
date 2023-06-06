using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace CoreDev.Extensions
{
    public static class SerializationHelper
    {
        private static BinaryFormatter binaryFormatter;
        private static SurrogateSelector surrogateSelector;
        public static BinaryFormatter BinaryFormatter
        {
            get
            {
                if (binaryFormatter == null)
                {
                    binaryFormatter = new BinaryFormatter();
                    surrogateSelector = new SurrogateSelector();
                    surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3SerializationSurrogate());
                    surrogateSelector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), new QuaternionSerializationSurrogate());
                    binaryFormatter.SurrogateSelector = surrogateSelector;
                }
                return binaryFormatter;
            }
        }

        public static void AddSurrogateExternal(Type type, StreamingContext context, ISerializationSurrogate surrogate)
        {
            BinaryFormatter binaryFormatterm = BinaryFormatter;
            surrogateSelector.AddSurrogate(type,context,surrogate);
        }

        public static byte[] Serialize(object obj)
        {
            if (obj != null)
            {
                try
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        SerializationHelper.BinaryFormatter.Serialize(memoryStream, obj);
                        return memoryStream.ToArray();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
            return null;
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            try
            {
                using (MemoryStream m = new MemoryStream(bytes))
                {
                    T obj = (T)SerializationHelper.BinaryFormatter.Deserialize(m);
                    return obj;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return default(T);
        }
    }

    public class Vector3SerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Vector3 vector3 = (Vector3)obj;
            info.AddValue("x", vector3.x);
            info.AddValue("y", vector3.y);
            info.AddValue("z", vector3.z);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Vector3 vector3 = (Vector3)obj;
            vector3.x = (float)info.GetValue("x", typeof(float));
            vector3.y = (float)info.GetValue("y", typeof(float));
            vector3.z = (float)info.GetValue("z", typeof(float));
            obj = vector3;
            return obj;
        }
    }

    public class QuaternionSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Quaternion quaternion = (Quaternion)obj;
            info.AddValue("x", quaternion.x);
            info.AddValue("y", quaternion.y);
            info.AddValue("z", quaternion.z);
            info.AddValue("w", quaternion.w);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Quaternion quaternion = (Quaternion)obj;
            quaternion.x = (float)info.GetValue("x", typeof(float));
            quaternion.y = (float)info.GetValue("y", typeof(float));
            quaternion.z = (float)info.GetValue("z", typeof(float));
            quaternion.w = (float)info.GetValue("w", typeof(float));
            obj = quaternion;
            return obj;
        }
    }
}