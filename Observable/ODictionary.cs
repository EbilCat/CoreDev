using System;
using System.Collections.Generic;
using CoreDev.Framework;
using UnityEngine;

namespace CoreDev.Observable
{
    public enum ODictionaryOperation { ADD, REMOVE };

    public class ODictionary<TKey, TValue> : Dictionary<TKey, TValue>, IObservableVar
    {
        public delegate bool ModerationCheck(ref TKey incomingKey, ref TValue incomingValue, OListOperation op);

        protected event Action ModeratorsChanged = delegate { };
        protected SortedList<int, List<ModerationCheck>> moderators = new SortedList<int, List<ModerationCheck>>();

        protected event Action ValueChanged = delegate { };
        protected IDataObject dataObject;

        public IDataObject DataObject
        {
            get { return dataObject; }
            set
            {
                if (dataObject != null)
                {
                    Debug.LogError("DataObject already assigned, all subsequent assignments are not allowed");
                    return;
                }
                this.dataObject = value;
            }
        }


//*====================
//* EVENT REGISTRATION
//*====================
        private event Action<ODictionary<TKey, TValue>, TKey, TValue> FireElementAddedCallback = delegate { };
        public void RegisterForElementAdded(Action<ODictionary<TKey, TValue>, TKey, TValue> callback, bool fireCallbackForExistingElements = true)
        {
            if (fireCallbackForExistingElements)
            {
                foreach (KeyValuePair<TKey, TValue> element in this)
                {
                    callback(this, element.Key, element.Value);
                }
            }
            FireElementAddedCallback -= callback;
            FireElementAddedCallback += callback;
        }

        public void UnregisterFromElementAdded(Action<ODictionary<TKey, TValue>, TKey, TValue> callback)
        {
            FireElementAddedCallback -= callback;
        }


        private event Action<ODictionary<TKey, TValue>, TKey, TValue> FireElementRemovedCallback = delegate { };
        public void RegisterForElementRemoved(Action<ODictionary<TKey, TValue>, TKey, TValue> callback)
        {
            FireElementRemovedCallback -= callback;
            FireElementRemovedCallback += callback;
        }

        public void UnregisterFromElementRemoved(Action<ODictionary<TKey, TValue>, TKey, TValue> callback)
        {
            FireElementRemovedCallback -= callback;
        }



//*====================
//* ACTIONS
//*====================
        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
        }

        public new bool Remove(TKey key)
        {
            return base.Remove(key);
        }

        public new void Clear()
        {
            base.Clear();
        }

        // public bool ContainsKey(TKey key)
        // {

        // }

        // public bool ContainsValue(TValue value)
        // {

        // }

        // public Enumerator GetEnumerator()
        // {

        // }

        // public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        // {

        // }

        // public virtual void OnDeserialization(object sender)
        // {

        // }

        // public bool TryGetValue(TKey key, out TValue value)
        // {

        // }


//*====================
//* QUERIES
//*====================
        public new TValue this[TKey key]
        {
            get
            {
                return base[key];
            }
            set
            {
                TValue itemToRemove = base[key];
                bool removalModerationPassed = this.ModerateIncomingValue(ref key, ref itemToRemove, OListOperation.REMOVE);
                bool additionModerationPassed = this.ModerateIncomingValue(ref key, ref value, OListOperation.ADD);

                if (removalModerationPassed && additionModerationPassed)
                {
                    this.FireElementRemovedCallback(this, key, itemToRemove);
                    base[key] = value;
                    this.ValueChanged();
                    this.FireElementAddedCallback(this, key, value);
                }
            }
        }

        public ODictionary<TKey, TValue> Value
        {
            get
            {
                return this;
            }
        }


//*====================
//* PRIVATE
//*====================
        private bool ModerateIncomingValue(ref TKey key, ref TValue value, OListOperation operation)
        {
            foreach (KeyValuePair<int, List<ModerationCheck>> kvp in moderators)
            {
                List<ModerationCheck> moderatorList = kvp.Value;

                for (int i = 0; i < moderatorList.Count; i++)
                {
                    bool isAcceptable = moderatorList[i](ref key, ref value, operation);
                    if (isAcceptable == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private List<ModerationCheck> GetModerationChecks(int priority)
        {
            if (moderators.ContainsKey(priority) == false)
            {
                this.moderators.Add(priority, new List<ModerationCheck>());
            }

            List<ModerationCheck> moderationChecks = this.moderators[priority];
            return moderationChecks;
        }
    }
}