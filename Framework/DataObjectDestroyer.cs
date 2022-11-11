using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreDev.Framework
{
    public class DataObjectDestroyer : MonoBehaviour
    {
        private List<IDataObject> dataObjectsToDestroy = new List<IDataObject>();


//*====================
//* STATIC
//*====================
        private static DataObjectDestroyer dataObjectDestroyer;
        public static DataObjectDestroyer Instance
        {
            get
            {
                if (dataObjectDestroyer == null && Application.isPlaying)
                {
                    GameObject dataObjectDestroyerGO = new GameObject("DataObjectDestroyer");
                    dataObjectDestroyer = dataObjectDestroyerGO.AddComponent<DataObjectDestroyer>();
                }
                return dataObjectDestroyer;
            }
        }

//*====================
//* UNITY
//*====================
        private void OnDestroy()
        {
            for (int i = dataObjectsToDestroy.Count - 1; i >= 0 ; i--)
            {
                dataObjectsToDestroy[0].Dispose();
                dataObjectsToDestroy.RemoveAt(0);
            }
        }


//*====================
//* PUBLIC
//*====================

        public void RegisterForDestructionOnSceneEnd(IDataObject dataObject)
        {
            this.dataObjectsToDestroy.Add(dataObject);
        }

        public void UnregisterFromDestructionOnSceneEnd(IDataObject dataObject)
        {
            this.dataObjectsToDestroy.Remove(dataObject);
        }
    }
}