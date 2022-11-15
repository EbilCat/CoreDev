using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreDev.Utils
{
    public class MainThreadManager : MonoBehaviour
    {
        private static MainThreadManager singleton;
        private static Queue<Action> mainThreadActions = new Queue<Action>();

        public static MainThreadManager Instance
        {
            get
            {
                if (singleton == null)
                {
                    GameObject mainThreadManager = new GameObject("MainThreadManager");
                    singleton = mainThreadManager.AddComponent<MainThreadManager>();
                    GameObject.DontDestroyOnLoad(mainThreadManager);
                }
                return singleton;
            }
        }


//*====================
//* UNITY
//*====================
        private void OnDestroy()
        {
            lock (mainThreadActions)
            {
                mainThreadActions.Clear();
            }
            singleton = null;
        }

        private void Update()
        {
            Action action;
            lock (mainThreadActions)
            {
                while (mainThreadActions.TryDequeue(out action))
                {
                    action?.Invoke();
                }
            }
        }


//*====================
//* PUBLIC
//*====================
        public void Run(Action action)
        {
            lock (mainThreadActions)
            {
                mainThreadActions.Enqueue(action);
            }
        }

        public void ClearActions()
        {
            lock (mainThreadActions)
            {
                mainThreadActions.Clear();
            }
        }
    }
}