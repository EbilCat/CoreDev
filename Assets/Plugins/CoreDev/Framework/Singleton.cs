using UnityEngine;

namespace CoreDev.Framework
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
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
                singletonInstance = (T)this;
                OnAwake();
            }
            else
            {
                Debug.LogErrorFormat("{0}: Singleton instance already existing at Awake. Destroying this gameobject {1}. Existing singleton instance is {2}."
                , this.GetType(), this.gameObject.name, singletonInstance.gameObject.name);
                Destroy(this.gameObject);
            }
        }
    }
}

