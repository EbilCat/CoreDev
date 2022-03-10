using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CoreDev.Framework
{
    public class Registry<U, T> : Singleton<Registry<U, T>> where T : class
    {
        public static event Action<U, T> registrantCreated = delegate { };
        public static event Action<U, T> removingRegistrant = delegate { };
        protected Dictionary<U, T> register = new Dictionary<U, T>();

        protected List<U> idList = new List<U>();
        protected List<T> registrantList = new List<T>();
        protected ReadOnlyCollection<U> readOnlyIds;
        protected ReadOnlyCollection<T> readOnlyRegistrants;

        //*===========================
        //* PUBLIC
        //*===========================
        public T GetRegistrant(U id)
        {
            // T registrant = register[id];
            T registrant = null;
            int index = idList.IndexOf(id);
            if (index != -1)
            {
                registrant = registrantList[index];
            }

            return registrant;
        }

        public bool RegistrantExists(U id)
        {
            bool exists = idList.Contains(id);
            return exists;
        }

        public List<U> ListRegistrantIds()
        {
            return new List<U>(idList);
        }


        public ReadOnlyCollection<U> GetIds()
        {
            if (readOnlyIds == null)
            {
                readOnlyIds = new ReadOnlyCollection<U>(idList);
            }
            return readOnlyIds;
        }

        public ReadOnlyCollection<T> GetRegistrants()
        {
            if (readOnlyRegistrants == null)
            {
                readOnlyRegistrants = new ReadOnlyCollection<T>(registrantList);
            }
            return readOnlyRegistrants;
        }

        public int RegistrantCount { get { return idList.Count; } }


        //*===========================
        //* PRIVATE
        //*===========================
        protected virtual bool AddRegistrant(U id, T registrant)
        {
            if (!RegistrantExists(id))
            {
                register.Add(id, registrant);
                idList.Add(id);
                registrantList.Add(registrant);

                registrantCreated(id, registrant);
                return true;
            }
            return false;
        }


        protected virtual bool RemoveRegistrant(U id)
        {
            if (RegistrantExists(id))
            {
                removingRegistrant(id, GetRegistrant(id));
                register.Remove(id);

                int index = idList.IndexOf(id);
                idList.Remove(id);
                registrantList.RemoveAt(index);

                return true;
            }
            return false;
        }
    }
}
