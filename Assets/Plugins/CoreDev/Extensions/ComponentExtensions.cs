using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreDev.Extensions
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// Recursively goes through the children of this component's gameobject to find the component with the specified name. 
        /// Logs error if it fails to locate the component.
        /// </summary>
        /// <remarks>
        /// Returns true if component successfully populated
        /// </remarks>
        public static bool TryGetComponentInChildren<T>(this Component thisComponent, out T component, string name, bool logErrorOnFailure = true) where T : Component
        {
            return thisComponent.gameObject.TryGetComponentInChildren(out component, name, logErrorOnFailure);
        }

        public static bool TryAssignment<T>(this Component thisComponent, out T assignee, T value) where T : class
        {
            assignee = value;
            return (assignee != null);
        }

        public static bool WarnIfAssignmentNull<T>(this Component thisComponent, out T assignee, T value) where T : class
        {
            assignee = value;
            return thisComponent.WarnIfNull(assignee);
        }

        public static bool LogErrorIfAssignmentNull<T>(this Component thisComponent, out T assignee, T value) where T : class
        {
            assignee = value;
            return thisComponent.LogErrorIfNull(assignee);
        }

        /// <summary>
        /// Convenience method to verify that a variable instance is not null. Logs warning if it is null.
        /// </summary>
        /// <remarks>
        /// Returns true if not null
        /// </remarks>
        public static bool WarnIfNull<T>(this Component thisComponent, T variable)
        {
            if (variable != null) { return true; }
            else
            {
                Debug.LogWarning(string.Format("{0}: {1} variable is null", thisComponent.GetType(), typeof(T)), thisComponent);
                return false;
            }
        }

        /// <summary>
        /// Convenience method to verify that a variable instance is not null. Logs error if it is null.
        /// </summary>
        /// <remarks>
        /// Returns true if not null
        /// </remarks>
        public static bool LogErrorIfNull<T>(this Component thisComponent, T variable)
        {
            if (variable != null) { return true; }
            else
            {
                Debug.LogError(string.Format("{0}: {1} variable is null", thisComponent.GetType(), typeof(T)), thisComponent);
                return false;
            }
        }
        /// <summary>
        /// Convenience method to check if the component's transform is already parented by the parent GameObject's transform
        /// and reparent the component if it is not
        /// </summary>
        public static void TrySetParent(this Component thisComponent, GameObject parentGO, bool worldPositionStays = true)
        {
            thisComponent.TrySetParent(parentGO.transform, worldPositionStays);
        }

        /// <summary>
        /// Convenience method to check if the component's transform is already parented by the parent's transform
        /// and reparent the component if it is not
        /// </summary>
        public static void TrySetParent(this Component thisComponent, Component parentComponent, bool worldPositionStays = true)
        {
            if (thisComponent == null) { Debug.LogWarning(string.Format("{0}: Unable to reparent component. It is null.", typeof(ComponentExtensions))); return; }
            if (parentComponent == null) { Debug.LogWarning(string.Format("{0}: Unable to reparent component {1} to parent. Parent is null.", typeof(ComponentExtensions), thisComponent.name)); return; }

            //Cache the transforms to reduce GetComponent() calls
            Transform thisTransform = (thisComponent is Transform) ? thisComponent as Transform : thisComponent.transform;
            Transform parentTransform = (parentComponent is Transform) ? parentComponent as Transform : parentComponent.transform;

            if (thisTransform.parent != parentTransform) //We try to avoid calls to SetParent cos costly.
            {
                thisTransform.SetParent(parentTransform, worldPositionStays);
            }
        }
    }
}
