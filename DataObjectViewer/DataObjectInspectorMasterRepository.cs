using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CoreDev.Framework;


namespace CoreDev.DataObjectInspector
{
    public class DataObjectInspectorMasterRepository
    {
        private static event Action<InspectedDataObjectDO> inspectedDataObjectDOCreated = delegate { };
        private static event Action<InspectedDataObjectDO> inspectedDataObjectDODisposing = delegate { };
        private static Dictionary<IDataObject, InspectedDataObjectDO> inspectedDataObjectDOMappings = new Dictionary<IDataObject, InspectedDataObjectDO>();
        private static List<InspectedDataObjectDO> inspectedDataObjectDOs = new List<InspectedDataObjectDO>();
        public static ReadOnlyCollection<InspectedDataObjectDO> InspectedDataObjectDOs { get { return inspectedDataObjectDOs.AsReadOnly(); } }

        public static void RegisterInspectedDataObjectDO(IDataObject dataObject, InspectedDataObjectDO inspectedDataObjectDO)
        {
            inspectedDataObjectDOMappings.Add(dataObject, inspectedDataObjectDO);
            inspectedDataObjectDOs.Add(inspectedDataObjectDO);
            inspectedDataObjectDOCreated(inspectedDataObjectDO);
        }

        public static void UnregisterInspectedDataObjectDO(IDataObject dataObject)
        {
            if (inspectedDataObjectDOMappings.ContainsKey(dataObject))
            {
                InspectedDataObjectDO inspectedDataObjectDO = inspectedDataObjectDOMappings[dataObject];
                inspectedDataObjectDODisposing(inspectedDataObjectDO);
                inspectedDataObjectDOMappings.Remove(dataObject);
                inspectedDataObjectDOs.Remove(inspectedDataObjectDO);
            }
        }

        public static void RegisterForCreation(Action<InspectedDataObjectDO> callback, bool fireCallbackForExisting = true)
        {
            UnregisterFromCreation(callback);
            inspectedDataObjectDOCreated += callback;

            if (fireCallbackForExisting)
            {
                foreach (InspectedDataObjectDO inspectedDataObjectDO in inspectedDataObjectDOs)
                {
                    callback(inspectedDataObjectDO);
                }
            }
        }

        public static void UnregisterFromCreation(Action<InspectedDataObjectDO> callback)
        {
            inspectedDataObjectDOCreated -= callback;
        }

        public static void RegisterForDisposing(Action<InspectedDataObjectDO> callback)
        {
            UnregisterFromDisposing(callback);
            inspectedDataObjectDODisposing += callback;
        }

        public static void UnregisterFromDisposing(Action<InspectedDataObjectDO> callback)
        {
            inspectedDataObjectDODisposing -= callback;
        }

        public static InspectedDataObjectDO GetInspectedDataObjectDO(IDataObject dataObject)
        {
            if (inspectedDataObjectDOMappings.ContainsKey(dataObject))
            {
                InspectedDataObjectDO inspectedDataObjectDO = inspectedDataObjectDOMappings[dataObject];
                return inspectedDataObjectDO;
            }
            return null;
        }

        public static bool ContainsEntry(IDataObject dataObject)
        {
            bool containsEntry = inspectedDataObjectDOMappings.ContainsKey(dataObject);
            return containsEntry;
        }
    }
}