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
                DataObjectMasterRepository.RegisterDataObject(singleton);
            }
            return singleton;
        }

        public static void DestroyDataObject()
        {
            singleton.Dispose();
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