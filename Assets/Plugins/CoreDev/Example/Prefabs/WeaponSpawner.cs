using UnityEngine;

public class WeaponSpawner : BaseSpawner<WeaponDO, Transform>
{
    protected override bool ShouldProcessDataObject(WeaponDO dataObject)
    {
        return true;
    }
    
    protected override Transform InstantiatePrefab(WeaponDO dataObject)
    {
        Transform prefabInstance = Instantiate(prefab);
        prefabInstance.SetParent(this.transform);
        return prefabInstance;
    }

    protected override void DisposePrefabInstance(Transform prefabInstance)
    {
        Destroy(prefabInstance.gameObject);
    }
}
