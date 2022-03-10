using UnityEngine;

namespace CoreDev.Framework
{
    /// <summary>
    /// Singleton that ensures the gameobject is not destroyed on scene unload. 
    /// This only applies when attached to a root gameobject.
    /// </summary>
    public abstract class MultipleSceneSingleton<T> : MonoBehaviour where T : MultipleSceneSingleton<T>
    {
        protected static T singletonInstance;

        public static T getInstance()
        {
            return singletonInstance;
        }

        /// <summary>
        /// Derived classes to use this method to perform Awake() actions
        /// </summary>
        protected virtual void OnAwake() { }

        protected void Awake()
        {
            if (getInstance() == null)
            {
                if (this.transform == this.transform.root)
                {
                    DontDestroyOnLoad(this.gameObject);
                }
            }

            /// <summary>
            /// Duplicate of Singleton<T>'s behaviour with error suppressed
            /// </summary>
            if (getInstance() == null)
            {
                singletonInstance = (T)this;
                OnAwake();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}
