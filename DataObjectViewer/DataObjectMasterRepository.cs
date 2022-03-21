﻿
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
        protected static Action<IDataObject> CreatedDO = delegate { };
        protected static Action<IDataObject> DisposingDO = delegate { };
        public static int DataObjectCount { get { return dataObjects.Count; } }


        //*===========================
        //* PUBLIC
        //*===========================
        public static void RegisterForCreation(Action<IDataObject> callback, bool fireCallbackForExistingDOs = true)
        {
            if (fireCallbackForExistingDOs)
            {
                for (int i = 0; i < dataObjects.Count; i++)
                {
                    IDataObject dataObject = dataObjects[i];
                    callback(dataObject);
                }
            }

            CreatedDO -= callback;
            CreatedDO += callback;
        }

        public static void UnregisterFromCreation(Action<IDataObject> callback)
        {
            CreatedDO -= callback;
        }

        public static void RegisterForDisposing(Action<IDataObject> callback)
        {
            DisposingDO -= callback;
            DisposingDO += callback;
        }

        public static void UnregisterFromDisposing(Action<IDataObject> callback, bool fireCallbackForExistingDOs = true)
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

        public static bool IsDataObjectRegistered(IDataObject dataObject)
        {
            bool exists = dataObjects.Contains(dataObject);
            return exists;
        }

        public static bool DestroyDataObject(IDataObject dataObject)
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
                dataObjects.Remove(dataObject);
                dataObject.Dispose();
                return true;
            }
            return false;
        }

        public static void DestroyAllDataObjects()
        {
            int lastDataObjIndex = dataObjects.Count - 1;

            while (lastDataObjIndex > 0)
            {
                IDataObject dataObject = dataObjects[lastDataObjIndex];
                DestroyDataObject(dataObject);
                lastDataObjIndex = dataObjects.Count - 1;
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

        public static bool RegisterDataObject(IDataObject dataObject)
        {
            DataObjectDestroyer.Instance?.RegisterForDestruction(DestroyAllDataObjects);

            if (!IsDataObjectRegistered(dataObject))
            {
                Type dataObjectType = dataObject.GetType();
                RegisterDataObjectByType(dataObject, dataObjectType);

                Type[] interfaces = dataObjectType.GetInterfaces();

                foreach (Type interfaceType in interfaces)
                {
                    RegisterDataObjectByType(dataObject, interfaceType);
                }

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
