using System;
using System.Collections.Generic;
using System.Reflection;
using CoreDev.Framework;
using CoreDev.Observable;

namespace CoreDev.DataObjectInspector
{
    public class InspectedDataObjectDO : IDataObject
    {
        private static Dictionary<Type, DataObjectInfoDO> dataObjectInfoDOs = new Dictionary<Type, DataObjectInfoDO>();
        private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

        private IDataObject dataObjectInstance;
        public IDataObject DataObjectInstance { get { return dataObjectInstance; } }

        private DataObjectInfoDO dataObjectInfoDO;
        public DataObjectInfoDO DataObjectInfoDO { get { return dataObjectInfoDO; } }

        public OString name;
        public OString observableVarFilterString;
        public OList<InspectedObservableVarDO> inspectedOVarDOs;
        public OBool matchesFilter;
        public OBool isInspected;
        public OEvent activateFilterTextField;


        public InspectedDataObjectDO(IDataObject dataObjectInstance)
        {
            this.dataObjectInstance = dataObjectInstance;

            this.name = new OString(dataObjectInstance.GetType().Name, this);
            this.observableVarFilterString = new OString(string.Empty, this);
            this.inspectedOVarDOs = new OList<InspectedObservableVarDO>(this);
            this.matchesFilter = new OBool(true, this);
            this.isInspected = new OBool(false, this);
            this.activateFilterTextField = new OEvent(this);

            Type dataObjectType = dataObjectInstance.GetType();
            if (dataObjectInfoDOs.ContainsKey(dataObjectType) == false)
            {
                DataObjectInfoDO dataObjectInfoDO = new DataObjectInfoDO(dataObjectType.GetAllFieldInfo(bindingFlags));
                dataObjectInfoDOs.Add(dataObjectType, dataObjectInfoDO);
            }

            this.dataObjectInfoDO = dataObjectInfoDOs[dataObjectType];
        }

        public void Inspect()
        {
            if (inspectedOVarDOs.Count == 0)
            {
                for (int i = 0; i < dataObjectInfoDO.observableVarInfos.Count; i++)
                {
                    ObservableVarInfoDO observableVarInfoDO = dataObjectInfoDO.observableVarInfos[i];

                    if (typeof(IObservableVar).IsAssignableFrom(observableVarInfoDO.FieldType))
                    {
                        IObservableVar oVarInstance = observableVarInfoDO.GetObservableVarInstance(dataObjectInstance);
                        if (oVarInstance != null)
                        {
                            InspectedObservableVarDO reflectedObservableVar = new InspectedObservableVarDO(this, oVarInstance, observableVarInfoDO, this.isInspected);
                            inspectedOVarDOs.Add(reflectedObservableVar);
                        }
                    }
                }
            }
        }


//*====================
//* IDataObject
//*====================
        public event Action<IDataObject> disposing;
        public void Dispose()
        {
            disposing?.Invoke(this);
            this.inspectedOVarDOs.Clear();
        }
    }
}