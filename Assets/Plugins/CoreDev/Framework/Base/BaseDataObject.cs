using CoreDev.Observable;
using UnityEngine;

namespace CoreDev.Framework
{
    public static class DataObjectBinder
    {
        public static void BindAspect(this IDataObject dataObject, Component aspectRoot)
        {
            ISpawnee[] spawnees = aspectRoot.GetComponentsInChildren<ISpawnee>(true);
            for (int i = 0; i < spawnees.Length; i++)
            {
                spawnees[i].BindDO(dataObject);
            }
        }

        public static void UnbindAspect(this IDataObject dataObject, Component aspectRoot)
        {
            ISpawnee[] spawnees = aspectRoot.GetComponentsInChildren<ISpawnee>(true);
            for (int i = 0; i < spawnees.Length; i++)
            {
                spawnees[i].UnbindDO(dataObject);
            }
        }

        public static void Claim(this IDataObject dataObject, params IObservableVar[] observableVar)
        {
            foreach (var item in observableVar)
            {
                if (item.DataObject == null) { item.DataObject = dataObject; }
            }
        }
    }

    public interface IDataObject 
    { 
        void Dispose();
    }
}