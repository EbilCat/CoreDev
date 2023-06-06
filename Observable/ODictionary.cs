﻿using System;
using System.Collections.Generic;
using CoreDev.Extensions;
using CoreDev.Framework;
using UnityEngine;

namespace CoreDev.Observable
{
    public enum ODictionaryOperation { ADD, REMOVE };

    public class ODictionary<TKey, TValue> : Dictionary<TKey, TValue>, IObservableVar
    {
        public delegate bool ModerationCheck(ref TKey incomingKey, ref TValue incomingValue, ODictionaryOperation op);

        protected event Action<string> ValueChangeBlocked = delegate { }; //This exists only for benefit of InspectedObservableVar
        protected event Action ValueChanged = delegate { }; //This exists only for benefit of InspectedObservableVar
        protected event Action ModeratorsChanged = delegate { }; //This exists only for benefit of InspectedObservableVar
        protected event Action CallbacksChanged = delegate { }; //This exists only for benefit of InspectedObservableVar

        protected SortedList<int, List<ModerationCheck>> moderators = new SortedList<int, List<ModerationCheck>>();

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

        public ODictionary(IDataObject dataObject = null) : base()
        {
            this.DataObject = dataObject;
        }


//*====================
//* MODERATORS
//*====================
        public void AddModerator(ModerationCheck acceptanceCheck, int priority = 0)
        {
            List<ModerationCheck> moderationChecks = GetModerationChecks(priority);
            moderationChecks.Remove(acceptanceCheck);
            moderationChecks.Add(acceptanceCheck);
            this.ModeratorsChanged();
        }

        public void RemoveModerator(ModerationCheck acceptanceCheck, int priority = 0)
        {
            List<ModerationCheck> moderationChecks = GetModerationChecks(priority);
            moderationChecks.Remove(acceptanceCheck);
            this.ModeratorsChanged();
        }


//*====================
//* EVENT REGISTRATION
//*====================
        private event Action<IObservableVar> ListeningCallbacks;
        public virtual void RegisterForChanges(Action<IObservableVar> callback, bool fireCallbackOnRegistration = true)
        {
            ListeningCallbacks -= callback;
            ListeningCallbacks += callback;

            if (fireCallbackOnRegistration)
            {
                try
                {
                    callback(this);
                }
                catch (Exception e)
                {
                    Debug.LogException(new Exception("ObservableVarCallbackException", e));
                }
            }
            this.CallbacksChanged();
        }

        public virtual void UnregisterFromChanges(Action<IObservableVar> callback)
        {
            ListeningCallbacks -= callback;
            this.CallbacksChanged();
        }


        private event Action<ODictionary<TKey, TValue>, TKey, TValue> FireElementAddedCallback;
        public virtual void RegisterForElementAdded(Action<ODictionary<TKey, TValue>, TKey, TValue> callback, bool fireCallbackForExistingElements = true)
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
            this.CallbacksChanged();
        }

        public virtual void UnregisterFromElementAdded(Action<ODictionary<TKey, TValue>, TKey, TValue> callback)
        {
            FireElementAddedCallback -= callback;
            this.CallbacksChanged();
        }


        private event Action<ODictionary<TKey, TValue>, TKey, TValue> FireElementRemovedCallback;
        public virtual void RegisterForElementRemoved(Action<ODictionary<TKey, TValue>, TKey, TValue> callback)
        {
            FireElementRemovedCallback -= callback;
            FireElementRemovedCallback += callback;
            this.CallbacksChanged();
        }

        public virtual void UnregisterFromElementRemoved(Action<ODictionary<TKey, TValue>, TKey, TValue> callback, bool fireCallbackForExistingElements = true)
        {
            if (fireCallbackForExistingElements)
            {
                foreach (KeyValuePair<TKey, TValue> element in this)
                {
                    callback(this, element.Key, element.Value);
                }
            }
            FireElementRemovedCallback -= callback;
            this.CallbacksChanged();
        }



//*====================
//* ACTIONS
//*====================
        public new bool Add(TKey key, TValue value)
        {
            bool moderationPassed = this.ModerateIncomingValue(ref key, ref value, ODictionaryOperation.ADD);
            if (moderationPassed)
            {
                base.Add(key, value);
                this.ValueChanged();
                try
                {
                    this.ListeningCallbacks?.Invoke(this);
                    this.FireElementAddedCallback?.Invoke(this, key, value);
                }
                catch (Exception e)
                {
                    Debug.LogException(new Exception("ObservableVarCallbackException", e));
                }
            }
            return moderationPassed;
        }

        public new bool Remove(TKey key)
        {
            TValue value;
            bool entryExists = base.TryGetValue(key, out value);
            bool isRemoved = false;

            if (entryExists)
            {
                bool moderationPassed = this.ModerateIncomingValue(ref key, ref value, ODictionaryOperation.REMOVE);
                if (moderationPassed)
                {
                    isRemoved = base.Remove(key);

                    if (isRemoved)
                    {
                        this.ValueChanged();
                        try
                        {
                            this.ListeningCallbacks?.Invoke(this);
                            this.FireElementRemovedCallback?.Invoke(this, key, value);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(new Exception("ObservableVarCallbackException", e));
                        }
                    }
                }
            }

            return isRemoved;
        }


        private List<TKey> keys = new List<TKey>();
        public new void Clear()
        {
            keys.Clear();
            keys.AddRange(base.Keys);
            int keyCount = this.Count;
            for (int i = 0; i < keyCount; i++)
            {
                Remove(keys[0]);
                keys.RemoveAt(0);
            }
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
                bool removalModerationPassed = this.ModerateIncomingValue(ref key, ref itemToRemove, ODictionaryOperation.REMOVE);
                bool additionModerationPassed = this.ModerateIncomingValue(ref key, ref value, ODictionaryOperation.ADD);

                if (removalModerationPassed && additionModerationPassed)
                {
                    try
                    {
                        this.FireElementRemovedCallback?.Invoke(this, key, itemToRemove);
                        base[key] = value;
                        this.ValueChanged();
                        this.ListeningCallbacks?.Invoke(this);
                        this.FireElementAddedCallback?.Invoke(this, key, value);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(new Exception("ObservableVarCallbackException", e));
                    }
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
        private bool ModerateIncomingValue(ref TKey key, ref TValue value, ODictionaryOperation operation)
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

        public string GetCallbacks()
        {
            string callbacks = string.Empty;

            Delegate[] anyChangeCallbackInvocationList = ListeningCallbacks?.GetInvocationList();
            if (anyChangeCallbackInvocationList != null)
            {
                foreach (Delegate invocation in anyChangeCallbackInvocationList)
                {
                    if (invocation.Target is MonoBehaviour)
                    {
                        MonoBehaviour monoBehaviour = invocation.Target as MonoBehaviour;
                        callbacks += $"ANY: {invocation.Target.GetType().Name} ({monoBehaviour.name})\r\n";
                    }
                    else
                    {
                        callbacks += $"ANY: {invocation.Target.GetType().Name}\r\n";
                    }
                }
            }

            Delegate[] elementAddedCallbackInvocationList = FireElementAddedCallback?.GetInvocationList();
            if (elementAddedCallbackInvocationList != null)
            {
                foreach (Delegate invocation in elementAddedCallbackInvocationList)
                {
                    if (invocation.Target is MonoBehaviour)
                    {
                        MonoBehaviour monoBehaviour = invocation.Target as MonoBehaviour;
                        callbacks += $"{invocation.Target.GetType().Name} ({monoBehaviour.name})\r\n";
                    }
                    else
                    {
                        callbacks += $"{invocation.Target.GetType().Name}\r\n";
                    }
                }
            }

            Delegate[] elementRemovedCallbackInvocationList = FireElementRemovedCallback?.GetInvocationList();
            if (elementRemovedCallbackInvocationList != null)
            {
                foreach (Delegate invocation in elementRemovedCallbackInvocationList)
                {
                    if (invocation.Target is MonoBehaviour)
                    {
                        MonoBehaviour monoBehaviour = invocation.Target as MonoBehaviour;
                        callbacks += $"{invocation.Target.GetType().Name} ({monoBehaviour.name})\r\n";
                    }
                    else
                    {
                        callbacks += $"{invocation.Target.GetType().Name}\r\n";
                    }
                }
            }

            return callbacks;
        }

        public void GetCallbacks(List<string> callbacks)
        {
            callbacks.Clear();

            Delegate[] anyChangeCallbackInvocationList = ListeningCallbacks?.GetInvocationList();
            if (anyChangeCallbackInvocationList != null)
            {
                foreach (Delegate invocation in anyChangeCallbackInvocationList)
                {
                    if (invocation.Target is MonoBehaviour)
                    {
                        MonoBehaviour monoBehaviour = invocation.Target as MonoBehaviour;
                        callbacks.Add($"ANY: {invocation.Target.GetType().Name} ({monoBehaviour.name})\r\n");
                    }
                    else
                    {
                        callbacks.Add($"ANY: {invocation.Target.GetType().Name}\r\n");
                    }
                }
            }


            Delegate[] elementAddedCallbackInvocationList = FireElementAddedCallback?.GetInvocationList();
            if (elementAddedCallbackInvocationList != null)
            {
                foreach (Delegate invocation in elementAddedCallbackInvocationList)
                {
                    if (invocation.Target is MonoBehaviour)
                    {
                        MonoBehaviour monoBehaviour = invocation.Target as MonoBehaviour;
                        callbacks.Add($"{invocation.Target.GetType().Name} ({monoBehaviour.name})");
                    }
                    else
                    {
                        callbacks.Add($"{invocation.Target.GetType().Name}");
                    }
                }
            }

            Delegate[] elementRemovedCallbackInvocationList = FireElementRemovedCallback?.GetInvocationList();
            if (elementRemovedCallbackInvocationList != null)
            {
                foreach (Delegate invocation in elementRemovedCallbackInvocationList)
                {
                    if (invocation.Target is MonoBehaviour)
                    {
                        MonoBehaviour monoBehaviour = invocation.Target as MonoBehaviour;
                        callbacks.Add($"{invocation.Target.GetType().Name} ({monoBehaviour.name})");
                    }
                    else
                    {
                        callbacks.Add($"{invocation.Target.GetType().Name}");
                    }
                }
            }
        }

        public virtual void SetValueFromString(string strVal)
        {
            Debug.LogWarning($"No override for SetValueFromString for type {this.GetType()}");
            if(strVal.ToLower().Contains("clear")||strVal.ToLower().Contains("clr"))
            {
                this.Clear();
            }
        }

        public byte[] ToBytes()
        {
            object[] kvps = new object[this.Count * 2];
            int count = 0;

            foreach (KeyValuePair<TKey, TValue> kvp in this)
            {
                kvps[count++] = kvp.Key;
                kvps[count++] = kvp.Value;
            }

            return SerializationHelper.Serialize(kvps);
        }

        public void SetValueFromBytes(byte[] bytes)
        {
            this.Clear();
            object[] objs = SerializationHelper.Deserialize<object[]>(bytes);

            for (int i = 0; i < objs.Length;)
            {
                TKey key = (TKey)objs[i++];
                TValue value = (TValue)objs[i++];
                this.Add(key, value);
            }
        }
    }
}