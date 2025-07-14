using System.Collections.Generic;
using UnityEngine;

namespace CoreDev.Framework
{
    public abstract class BaseSpawner<DO, PrefabType> : BaseSpawnee
        where DO : class, IDataObject
        where PrefabType : Component
    {
        protected Dictionary<DO, PrefabType> prefabInstances = new Dictionary<DO, PrefabType>();

        [SerializeField]
        protected PrefabType prefab;
    
    
//*====================
//* BINDING
//*====================    
        protected override void RegisterCallbacks()
        {
            base.RegisterCallbacks();
            DataObjectMasterRepository.RegisterForCreation(DataObjectCreated);
            DataObjectMasterRepository.RegisterForDisposing(DataObjectDisposing);
        }
    
        protected override void UnregisterCallbacks()
        {
            base.UnregisterCallbacks();
            DataObjectMasterRepository.UnregisterFromCreation(DataObjectCreated);
            DataObjectMasterRepository.UnregisterFromDisposing(DataObjectDisposing);
        }


//*===========================
//* PRIVATE
//*===========================
        protected virtual void DataObjectCreated(IDataObject dataObject)
        {
            DO expectedDO = dataObject as DO;

            if (expectedDO != null && ShouldProcessDataObject(expectedDO))
            {
                PrefabType prefabinstance = InstantiatePrefab(expectedDO);
                expectedDO.BindAspect(prefabinstance);
                prefabInstances.Add(expectedDO, prefabinstance);
            }
        }

        protected virtual void DataObjectDisposing(IDataObject dataObject)
        {
            DO expectedDO = dataObject as DO;

            if (expectedDO != null && prefabInstances.ContainsKey(expectedDO))
            {
                if (ShouldProcessDataObject(expectedDO))
                {
                    PrefabType prefabInstance = prefabInstances[expectedDO];
                    if (prefabInstance != null)
                    {
                        expectedDO.UnbindAspect(prefabInstance);
                        prefabInstances.Remove(expectedDO);
                        DisposePrefabInstance(prefabInstance);
                    }
                }
            }
        }

        protected abstract bool ShouldProcessDataObject(DO dataObject);
        protected abstract PrefabType InstantiatePrefab(DO dataObject);
        protected abstract void DisposePrefabInstance(PrefabType prefabInstance);
    }
}