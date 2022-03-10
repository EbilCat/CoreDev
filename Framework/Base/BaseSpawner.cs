using System.Collections.Generic;
using CoreDev.Framework;
using UnityEngine;

public abstract class BaseSpawner<DO, PrefabType> : MonoBehaviour 
    where DO : class, IDataObject
    where PrefabType : Component
{
    protected Dictionary<DO, PrefabType> prefabInstances = new Dictionary<DO, PrefabType>();

    [SerializeField]
    protected PrefabType prefab;


//*===========================
//* UNITY
//*===========================
    protected virtual void Awake()
    {
        DataObjectMasterRepository.RegisterForCreation(DataObjectCreated);
        DataObjectMasterRepository.RegisterForDisposing(DataObjectDisposing);
    }

    protected virtual void OnDestroy()
    {
        DataObjectMasterRepository.UnregisterFromCreation(DataObjectCreated);
        DataObjectMasterRepository.UnregisterFromDisposing(DataObjectDisposing);
    }


//*===========================
//* PRIVATE
//*===========================
    private void DataObjectCreated(IDataObject dataObject)
    {
        DO expectedDO = dataObject as DO;
        
        if (expectedDO != null && ShouldProcessDataObject(expectedDO))
        {
            PrefabType prefabinstance = InstantiatePrefab(expectedDO);
            expectedDO.BindAspect(prefabinstance);
            prefabInstances.Add(expectedDO, prefabinstance);
        }
    }

    private void DataObjectDisposing(IDataObject dataObject)
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