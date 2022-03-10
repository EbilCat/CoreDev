using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CoreDev.Framework;

namespace CoreDev.Observable
{
    //*===========================
    //* ODictionary
    //*===========================
    public class ODictionary<DO, KEY, VAL> where DO : IDataObject
    {
        private Dictionary<KEY, VAL> var = new Dictionary<KEY, VAL>();
        private ReadOnlyDictionary<KEY, VAL> readOnlyVar;
        private DO dataObject;
        public DO DataObject
        {
            get { return dataObject; }
        }
        private event Action<ODictionary<DO, KEY, VAL>> DictionaryAltered = delegate { }; //Dictionary
        private event Action<ODictionary<DO, KEY, VAL>, KEY, VAL> ElementAdded = delegate { }; //Dictionary, Key, Value
        private event Action<ODictionary<DO, KEY, VAL>, KEY, VAL> RemovingElement = delegate { }; //Dictionary, Key, Value

        public ReadOnlyDictionary<KEY, VAL> Value
        {
            get { return this.readOnlyVar; }
        }

        public int Count
        {
            get { return var.Count; }
        }

        public void Add(KEY key, VAL item)
        {
            this.var.Add(key, item);
            this.DictionaryAltered(this);
            this.ElementAdded(this, key, item);
        }

        public bool Remove(KEY key)
        {
            bool containsKey = this.var.ContainsKey(key);

            if (containsKey)
            {
                this.var.Remove(key);
                this.DictionaryAltered(this);
            }

            return containsKey;
        }

        public void Clear()
        {
            if (this.var.Count > 0)
            {
                foreach (KeyValuePair<KEY, VAL> entry in var)
                {
                    this.RemovingElement(this, entry.Key, entry.Value);
                }
                this.var.Clear();
                this.DictionaryAltered(this);
            }
        }

        public void RegisterForChanges(Action<ODictionary<DO, KEY, VAL>> callback, bool fireCallbackOnRegistration = true)
        {
            if (fireCallbackOnRegistration) { callback(this); }
            DictionaryAltered -= callback;
            DictionaryAltered += callback;
        }

        public void RegisterForElementAdded(Action<ODictionary<DO, KEY, VAL>, KEY, VAL> callback, bool fireCallbackOnRegistration = true) //Dictionary, Index, Count
        {
            if (fireCallbackOnRegistration && this.var.Count > 0)
            {
                foreach (KeyValuePair<KEY, VAL> entry in var)
                {
                    callback(this, entry.Key, entry.Value);
                }
            }
            ElementAdded -= callback;
            ElementAdded += callback;
        }

        public void RegisterForRemovingElement(Action<ODictionary<DO, KEY, VAL>, KEY, VAL> callback) //Dictionary, Index, Count
        {
            RemovingElement -= callback;
            RemovingElement += callback;
        }

        public void UnregisterFromChanges(Action<ODictionary<DO, KEY, VAL>> callback) { DictionaryAltered -= callback; }
        public void UnregisterFromElementAdded(Action<ODictionary<DO, KEY, VAL>, KEY, VAL> callback) { ElementAdded -= callback; }
        public void UnregisterFromRemovingElement(Action<ODictionary<DO, KEY, VAL>, KEY, VAL> callback) { RemovingElement -= callback; }


        public ODictionary(DO dataObject)
        {
            this.dataObject = dataObject;
            this.readOnlyVar = new ReadOnlyDictionary<KEY, VAL>(var);
        }
    }
}