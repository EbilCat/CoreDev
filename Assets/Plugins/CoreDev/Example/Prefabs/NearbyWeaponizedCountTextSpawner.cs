using CoreDev.Examples;
using UnityEngine;

public class NearbyWeaponizedCountTextSpawner : BaseSpawner<AngryCubeDO, RectTransform>
{
    protected override void DisposePrefabInstance(RectTransform prefabInstance)
    {
        Destroy(prefabInstance.gameObject);
    }

    protected override RectTransform InstantiatePrefab(AngryCubeDO dataObject)
    {
        RectTransform prefabInstance = Instantiate(prefab);

        prefabInstance.SetParent(this.transform);
        prefabInstance.localPosition = Vector3.zero;
        prefabInstance.localRotation = Quaternion.identity;
        prefabInstance.localScale = Vector3.one;

        return prefabInstance;
    }

    protected override bool ShouldProcessDataObject(AngryCubeDO dataObject)
    {
        return true;
    }
}
