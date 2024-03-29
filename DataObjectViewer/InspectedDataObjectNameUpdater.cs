﻿using System;
using System.Collections.ObjectModel;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;

namespace CoreDev.DataObjectInspector
{
    public class InspectedDataObjectNameUpdater : MonoBehaviour, ISpawnee
    {
        private InspectedDataObjectDO inspectedDataObjectDO;
        private ReadOnlyCollection<ObservableVarInfoDO> observableVarInfos;


//*====================
//* UNITY
//*====================
        private void OnDestroy()
        {
            if (observableVarInfos != null)
            {
                foreach (ObservableVarInfoDO observableVarInfoDO in observableVarInfos)
                {
                    IObservableVar observableVarInstance = observableVarInfoDO.GetObservableVarInstance(this.inspectedDataObjectDO.DataObjectInstance);
                    observableVarInfoDO.UnregisterFromValueChanges(observableVarInstance, RefreshName);
                    observableVarInfoDO.isBookedMarked.UnregisterFromChanges(OnIsBookedMarkedChanged);
                }
            }
        }


//*====================
//* BINDING
//*====================
        public void BindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedDataObjectDO)
            {
                UnbindDO(this.inspectedDataObjectDO);
                this.inspectedDataObjectDO = dataObject as InspectedDataObjectDO;
                this.observableVarInfos = this.inspectedDataObjectDO.DataObjectInfoDO.observableVarInfos.Value;

                foreach (ObservableVarInfoDO observableVarInfoDO in observableVarInfos)
                {
                    observableVarInfoDO.isBookedMarked.RegisterForChanges(OnIsBookedMarkedChanged);
                }
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedDataObjectDO && this.inspectedDataObjectDO == dataObject)
            {
                foreach (ObservableVarInfoDO observableVarInfoDO in observableVarInfos)
                {
                    IObservableVar observableVarInstance = observableVarInfoDO.GetObservableVarInstance(this.inspectedDataObjectDO.DataObjectInstance);
                    observableVarInfoDO.UnregisterFromValueChanges(observableVarInstance, RefreshName);
                    observableVarInfoDO.isBookedMarked.UnregisterFromChanges(OnIsBookedMarkedChanged);
                }

                this.inspectedDataObjectDO = null;
                this.observableVarInfos = null;
            }
        }


//*====================
//* CALLBACKS - ObservableVarInfoDO
//*====================
        private void OnIsBookedMarkedChanged(ObservableVar<bool> oIsBookedMarked)
        {
            ObservableVarInfoDO observableVarInfoDO = oIsBookedMarked.DataObject as ObservableVarInfoDO;
            IObservableVar observableVarInstance = observableVarInfoDO.GetObservableVarInstance(this.inspectedDataObjectDO.DataObjectInstance);

            if (observableVarInstance != null)
            {
                if (oIsBookedMarked.Value)
                {
                    observableVarInfoDO.RegisterForValueChanges(observableVarInstance, RefreshName);
                }
                else
                {
                    observableVarInfoDO.UnregisterFromValueChanges(observableVarInstance, RefreshName);
                }
            }
            this.RefreshName();
        }

        private void RefreshName()
        {
            string name = this.inspectedDataObjectDO.DataObjectInstance.GetType().Name;

            foreach (ObservableVarInfoDO observableVarInfoDO in observableVarInfos)
            {
                if (observableVarInfoDO.isBookedMarked.Value)
                {
                    IObservableVar observableVarInstance = observableVarInfoDO.GetObservableVarInstance(this.inspectedDataObjectDO.DataObjectInstance);

                    if (observableVarInstance != null)
                    {
                        name += $"{Environment.NewLine}{observableVarInfoDO.Name}:{observableVarInstance}";
                    }
                }
            }

            this.inspectedDataObjectDO.name.Value = name;
        }
    }
}