﻿using System;
using System.Collections.Generic;
using System.Reflection;
using CoreDev.Framework;
using CoreDev.Observable;

public class InspectedDataObjectDO : IDataObject
{
    private static Dictionary<Type, DataObjectInfoDO> dataObjectInfoDOs = new Dictionary<Type, DataObjectInfoDO>();
    private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

    private IDataObject dataObjectInstance;
    public IDataObject DataObjectInstance { get { return dataObjectInstance; } }

    private DataObjectInfoDO dataObjectInfoDO;
    public DataObjectInfoDO DataObjectInfoDO { get { return dataObjectInfoDO; } }

    public OString name;
    public OList<InspectedObservableVarDO> inspectedOVarDOs;
    public OBool matchesFilter;
    public OBool isInspected;


    public InspectedDataObjectDO(IDataObject dataObjectInstance)
    {
        this.dataObjectInstance = dataObjectInstance;

        this.name = new OString(dataObjectInstance.GetType().Name, this);
        this.inspectedOVarDOs = new OList<InspectedObservableVarDO>(this);
        this.matchesFilter = new OBool(true, this);
        this.isInspected = new OBool(false, this);

        Type dataObjectType = dataObjectInstance.GetType();
        if (dataObjectInfoDOs.ContainsKey(dataObjectType) == false)
        {
            DataObjectInfoDO newAttributes = new DataObjectInfoDO(dataObjectType.GetAllFieldInfo(bindingFlags));
            dataObjectInfoDOs.Add(dataObjectType, newAttributes);
        }

        this.dataObjectInfoDO = dataObjectInfoDOs[dataObjectType];

        for (int i = 0; i < dataObjectInfoDO.observableVarInfos.Count; i++)
        {
            ObservableVarInfoDO observableVarInfoDO = dataObjectInfoDO.observableVarInfos[i];

            if (typeof(IObservableVar).IsAssignableFrom(observableVarInfoDO.FieldType))
            {
                IObservableVar oVarInstance = observableVarInfoDO.GetObservableVarInstance(dataObjectInstance);
                InspectedObservableVarDO reflectedObservableVar = new InspectedObservableVarDO(oVarInstance, observableVarInfoDO, this.isInspected);
                inspectedOVarDOs.Add(reflectedObservableVar);
            }
        }
    }

        
//*====================
//* IDataObject
//*====================
        public void Dispose() { }
}