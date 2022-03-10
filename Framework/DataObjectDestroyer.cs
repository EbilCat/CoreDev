using System;
using UnityEngine;

namespace CoreDev.Framework
{
    public class DataObjectDestroyer : MonoBehaviour
    {
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
            this.destructorCallback();
            DataObjectMasterRepository.DestroyAllDataObjects();
        }


//*====================
//* PUBLIC
//*====================
        private event Action destructorCallback = delegate { };

        public void RegisterForDestruction(Action destructorCallback)
        {
            this.destructorCallback -= destructorCallback;
            this.destructorCallback += destructorCallback;
        }

        public void UnregisterFromDestruction(Action destructorCallback)
        {
            this.destructorCallback -= destructorCallback;
        }
    }
}