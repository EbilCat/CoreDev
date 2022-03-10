using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CoreDev.Extensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Recursively goes through the children of this gameobject to find the component with the specified name. 
        /// Logs error if it fails to locate the component.
        /// </summary>
        /// <remarks>
        /// Returns true if component successfully populated
        /// </remarks>
        public static bool TryGetComponentInChildren<T>(this GameObject go, out T component, string name, bool logErrorOnFailure = true) where T : Component
        {
            component = go.FindRecursive<T>(name);
            if (component == null)
            {
                string message = string.Format("{0}: Could not locate component {1} on GameObject {2}", go.name, typeof(T), name);
                if (logErrorOnFailure) { Debug.LogError(message); }
                else { Debug.LogWarning(message); }
            }
            return component != null;
        }

        /// <summary>
        /// Convenience method to check if the gameobject's transform is already parented by the parent GameObject's transform
        /// and reparent the gameobject if it is not
        /// </summary>
        public static void TrySetParent(this GameObject thisGO, GameObject parentGO, bool worldPositionStays = true)
        {
            thisGO.transform.TrySetParent(parentGO, worldPositionStays);
        }

        /// <summary>
        /// Convenience method to check if the gameobject's transform is already parented by the parent component's transform
        /// and reparent the gameobject if it is not
        /// </summary>
        public static void TrySetParent(this GameObject thisGO, Component parentGO, bool worldPositionStays = true)
        {
            thisGO.transform.TrySetParent(parentGO, worldPositionStays);
        }

        public static GameObject FindRecursive(this GameObject go, string name)
        {
            // GameObject foundGO = (from x in go.GetComponentsInChildren<Transform>(true)
            //                       where x.gameObject.name == name
            //                       select x.gameObject).First();

            // return foundGO;

            Transform[] transforms = go.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < transforms.Length; i++)
            {
                Transform tChild = transforms[i];
                if (tChild.name == name)
                {
                    return tChild.gameObject;
                }
            }

            return null;
        }

        public static GameObject FindRecursiveEndingWith(this GameObject go, string name)
        {
            // GameObject foundGO = (from x in go.GetComponentsInChildren<Transform>(true)
            //                       where x.gameObject.name == name
            //                       select x.gameObject).First();

            // return foundGO;

            Transform[] transforms = go.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < transforms.Length; i++)
            {
                Transform tChild = transforms[i];
                if (tChild.name.EndsWith(name))
                {
                    return tChild.gameObject;
                }
            }

            return null;
        }

        public static GameObject FindRecursiveStartingWith(this GameObject go, string name)
        {
            // GameObject foundGO = (from x in go.GetComponentsInChildren<Transform>(true)
            //                       where x.gameObject.name == name
            //                       select x.gameObject).First();

            // return foundGO;

            Transform[] transforms = go.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < transforms.Length; i++)
            {
                Transform tChild = transforms[i];
                if (tChild.name.StartsWith(name))
                {
                    return tChild.gameObject;
                }
            }

            return null;
        }

        public static T FindRecursive<T>(this GameObject go, string name) where T : Component
        {
            GameObject foundGO = go.FindRecursive(name);
            T foundComponent = null;
            if (foundGO != null) { foundComponent = foundGO.GetComponent<T>(); }
            return foundComponent;
        }

        public static T FindRecursiveEndingWith<T>(this GameObject go, string name) where T : Component
        {
            GameObject foundGO = go.FindRecursiveEndingWith(name);
            T foundComponent = null;
            if (foundGO != null) { foundComponent = foundGO.GetComponent<T>(); }
            return foundComponent;
        }

        public static T FindRecursiveStartingWith<T>(this GameObject go, string name) where T : Component
        {
            GameObject foundGO = go.FindRecursiveStartingWith(name);
            T foundComponent = null;
            if (foundGO != null) { foundComponent = foundGO.GetComponent<T>(); }
            return foundComponent;
        }

        public static T FindRecursive<T>(string name) where T : Component
        {
            GameObject foundGO = GameObject.Find(name);
            T foundComponent = null;
            if (foundGO != null) { foundComponent = foundGO.GetComponent<T>(); }
            return foundComponent;
        }

        public static Vector3 GetCentrePos(this List<GameObject> gameObjects)
        {
            GameObject firstGO = gameObjects[0];
            Vector3 centerPos = firstGO.transform.position;

            for (int i = 1; i < gameObjects.Count; i++)
            {
                GameObject currentGO = gameObjects[i];
                centerPos += currentGO.transform.position;
                // formationPos = ((currentGO.transform.position - formationPos ) / 2) + formationPos;
            }

            centerPos /= gameObjects.Count;
            return centerPos;
        }


        private static List<RaycastResult> eventSystemRaycastResults = new List<RaycastResult>();
        private static List<GameObject> potentialHandlerGOs = new List<GameObject>();
        private static List<GameObject> previousFallthroughs = new List<GameObject>();
        public static GameObject BubbleEvent<T>(this GameObject fallthrough, PointerEventData eventData, ExecuteEvents.EventFunction<T> functor, int bubbleLimit = -1) where T : IEventSystemHandler
        {
            bool isFirstFallthroughObject = previousFallthroughs.Count == 0;
            
            if (isFirstFallthroughObject) //If 0, indicates that this is the first fallthrough object
            {
                EventSystem.current.RaycastAll(eventData, eventSystemRaycastResults); //Get all objects under 
                BuildPotentialHandlerList();
                previousFallthroughs.Add(fallthrough);
            }

            GameObject handlerGO = null;

            while (0 < potentialHandlerGOs.Count && (bubbleLimit == -1 || previousFallthroughs.Count <= bubbleLimit))
            {
                GameObject potentialHandlerGO = potentialHandlerGOs[0];
                potentialHandlerGOs.RemoveAt(0);

                handlerGO = ExecuteEvents.GetEventHandler<T>(potentialHandlerGO);

                if (handlerGO != null && HasNotBeenExecuted(handlerGO))
                {
                    previousFallthroughs.Add(handlerGO);
                    ExecuteEvents.Execute<T>(handlerGO, eventData, functor);
                    if (handlerGO != null) { break; }
                }
            }

            if (isFirstFallthroughObject)
            {
                eventSystemRaycastResults.Clear();
                potentialHandlerGOs.Clear();
                previousFallthroughs.Clear();
            }

            return handlerGO;
        }

        private static void BuildPotentialHandlerList()
        {
            for (int i = 0; i < eventSystemRaycastResults.Count; i++)
            {
                RaycastResult result = eventSystemRaycastResults[i];
                GameObject potentialHandlerGO = result.gameObject;
                potentialHandlerGOs.Add(potentialHandlerGO);
                Transform t = potentialHandlerGO.transform.parent;
                while (t != null)
                {
                    if (potentialHandlerGOs.Contains(t.gameObject) == false)
                    {
                        potentialHandlerGOs.Add(t.gameObject);
                    }
                    t = t.parent;
                }
            }
        }

        private static bool HasNotBeenExecuted(GameObject handlerGO)
        {
            bool hasNotBeenExecuted = previousFallthroughs.Contains(handlerGO) == false;
            return hasNotBeenExecuted;
        }
    }
}