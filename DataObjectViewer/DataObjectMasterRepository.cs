using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace CoreDev.Framework
{
    public class DataObjectMasterRepository
    {
        //* Static
        protected static List<IDataObject> dataObjects = new List<IDataObject>();
        protected static Dictionary<Type, List<IDataObject>> typeSegregatedDOs = new Dictionary<Type, List<IDataObject>>();
        protected static event Action<IDataObject> CreatedDO = delegate { };
        protected static event Action<IDataObject> DisposingDO = delegate { };
        public static int DataObjectCount { get { return dataObjects.Count; } }


        //*===========================
        //* PUBLIC
        //*===========================
        public static void RegisterForCreation(Action<IDataObject> callback, bool fireCallbackForExistingDOs = true)
        {
            if (IsCreationCallbackRegistered(callback) == false)
            {
                if (fireCallbackForExistingDOs)
                {
                    for (int i = 0; i < dataObjects.Count; i++)
                    {
                        IDataObject dataObject = dataObjects[i];
                        callback(dataObject);
                    }
                }
                CreatedDO += callback;
            }
        }

        public static void UnregisterFromCreation(Action<IDataObject> callback)
        {
            CreatedDO -= callback;
        }

        public static bool IsCreationCallbackRegistered(Action<IDataObject> callback)
        {
            Delegate[] invocationList = CreatedDO.GetInvocationList();
            foreach (Delegate invocation in invocationList)
            {
                if (invocation == callback as Delegate)
                {
                    return true;
                }
            }
            return false;
        }


        public static void RegisterForDisposing(Action<IDataObject> callback)
        {
            DisposingDO -= callback;
            DisposingDO += callback;
        }

        public static void UnregisterFromDisposing(Action<IDataObject> callback, bool fireCallbackForExistingDOs = true)
        {
            if (IsDisposingCallbackRegistered(callback))
            {
                if (fireCallbackForExistingDOs)
                {
                    for (int i = 0; i < dataObjects.Count; i++)
                    {
                        IDataObject dataObject = dataObjects[i];
                        callback(dataObject);
                    }
                }
                DisposingDO -= callback;
            }
        }

        public static bool IsDisposingCallbackRegistered(Action<IDataObject> callback)
        {
            Delegate[] invocationList = DisposingDO.GetInvocationList();
            foreach (Delegate invocation in invocationList)
            {
                if (invocation == callback as Delegate)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsDataObjectRegistered(IDataObject dataObject)
        {
            bool exists = dataObjects.Contains(dataObject);
            return exists;
        }

        private static void DestroyDataObject(IDataObject dataObject)
        {
            if (IsDataObjectRegistered(dataObject))
            {
                Type dataObjectType = dataObject.GetType();
                UnregisterDataObjectByType(dataObject, dataObjectType);

                Type[] interfaces = dataObjectType.GetInterfaces();

                foreach (Type interfaceType in interfaces)
                {
                    UnregisterDataObjectByType(dataObject, interfaceType);
                }

                DisposingDO(dataObject);
                dataObject.disposing -= DestroyDataObject;
                dataObjects.Remove(dataObject);
            }
        }

        public static ReadOnlyCollection<IDataObject> GetDataObjects()
        {
            return dataObjects.AsReadOnly();
        }

        public static ReadOnlyCollection<IDataObject> GetDataObjects<T>()
        {
            List<IDataObject> typeSpecificList = null;
            bool listExists = typeSegregatedDOs.TryGetValue(typeof(T), out typeSpecificList);
            if (listExists == false)
            {
                typeSpecificList = new List<IDataObject>();
                typeSegregatedDOs.Add(typeof(T), typeSpecificList);
            }
            return typeSpecificList.AsReadOnly();
        }

        public static T GetDataObject<T>(Predicate<T> filterCondition = null) where T : class, IDataObject
        {
            List<IDataObject> typeSpecificList = null;
            bool listExists = typeSegregatedDOs.TryGetValue(typeof(T), out typeSpecificList);
            if (listExists == false)
            {
                typeSpecificList = new List<IDataObject>();
                typeSegregatedDOs.Add(typeof(T), typeSpecificList);
            }

            foreach (IDataObject item in typeSpecificList)
            {
                T typedItem = (T)item;

                if (filterCondition == null || filterCondition(typedItem))
                {
                    return typedItem;
                }
            }
            return null;
        }

        public static void GetDataObjects<T>(List<T> listToPopulate, Predicate<T> filterCondition = null)
        {
            listToPopulate.Clear();

            List<IDataObject> typeSpecificList = null;
            bool listExists = typeSegregatedDOs.TryGetValue(typeof(T), out typeSpecificList);
            if (listExists == false)
            {
                typeSpecificList = new List<IDataObject>();
                typeSegregatedDOs.Add(typeof(T), typeSpecificList);
            }

            foreach (IDataObject item in typeSpecificList)
            {
                T typedItem = (T)item;

                if (filterCondition == null || filterCondition(typedItem))
                {
                    listToPopulate.Add(typedItem);
                }
            }
        }

        public static bool RegisterDataObject(IDataObject dataObject, bool destroyOnSceneEnd = true)
        {
            if (dataObject == null)
            {
                Debug.LogWarning("Rejecting attempt to register a NULL DataObject");
                return false;
            }

            if (!IsDataObjectRegistered(dataObject))
            {
                if (destroyOnSceneEnd)
                {
                    DataObjectDestroyer.Instance.RegisterForDestructionOnSceneEnd(dataObject);
                }

                Type dataObjectType = dataObject.GetType();
                RegisterDataObjectByType(dataObject, dataObjectType);

                Type[] interfaces = dataObjectType.GetInterfaces();

                foreach (Type interfaceType in interfaces)
                {
                    RegisterDataObjectByType(dataObject, interfaceType);
                }

                dataObject.disposing += DestroyDataObject;
                dataObjects.Add(dataObject);
                CreatedDO(dataObject);
                return true;
            }
            return false;
        }

        private static void RegisterDataObjectByType(IDataObject dataObject, Type currentType)
        {
            List<IDataObject> typeSpecificList = null;
            bool listExists = typeSegregatedDOs.TryGetValue(currentType, out typeSpecificList);
            if (listExists == false)
            {
                typeSpecificList = new List<IDataObject>();
                typeSegregatedDOs.Add(currentType, typeSpecificList);
            }
            typeSpecificList.Add(dataObject);
        }

        private static void UnregisterDataObjectByType(IDataObject dataObject, Type currentType)
        {
            List<IDataObject> typeSpecificList = null;
            bool listExists = typeSegregatedDOs.TryGetValue(currentType, out typeSpecificList);

            if (listExists == true)
            {
                typeSpecificList.Remove(dataObject);
            }
        }
    }
}

