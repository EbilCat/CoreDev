using UnityEngine;


namespace CoreDev.Examples
{
    public class CubeSpawner : BaseSpawner<AngryCubeDO, Transform>
    {
        [SerializeField] private int teamId;

        protected override bool ShouldProcessDataObject(AngryCubeDO dataObject)
        {
            bool shouldProcess = (dataObject.teamId.Value == this.teamId);
            return shouldProcess;
        }

        protected override Transform InstantiatePrefab(AngryCubeDO dataObject)
        {
            Transform prefabInstance = Instantiate(this.prefab);
            prefabInstance.transform.SetParent(this.transform);
            return prefabInstance;
        }

        protected override void DisposePrefabInstance(Transform prefabInstance)
        {
            Destroy(prefabInstance.gameObject);
        }
    }
}