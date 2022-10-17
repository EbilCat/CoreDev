using System;

namespace CoreDev.Framework
{
    public abstract class SingletonDataObject<DO> : IDataObject where DO : class, IDataObject, new()
    {
        private static DO singleton = null;

        public static DO GetInstance()
        {
            if (singleton == null)
            {
                singleton = new DO();
                DataObjectDestroyer.Instance?.RegisterForDestruction(DestroyDataObject);
                DataObjectMasterRepository.RegisterDataObject(singleton);
            }
            return singleton;
        }

        public static void DestroyDataObject()
        {
            DataObjectMasterRepository.DestroyDataObject(singleton);
            singleton = null;
        }


//*====================
//* IDataObject
//*====================
        public event Action<IDataObject> disposing;
        public void Dispose()
        {
            disposing?.Invoke(this);
        }
    }
}