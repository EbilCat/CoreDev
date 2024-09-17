using System;
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

        private event Action<IObservableVar> AnyChangeCallback;
        private event Action<ODictionary<TKey, TValue>, TKey, TValue> FireElementAddedCallback;
        private event Action<ODictionary<TKey, TValue>, TKey, TValue> FireElementRemovedCallback;
        private Delegate[] anyChangeCallback_Invocations = null;
        private Delegate[] fireElementAddedCallback_Invocations = null;
        private Delegate[] fireElementRemovedCallback_Invocations = null;

        protected SortedList<int, List<ModerationCheck>> moderators = new SortedList<int, List<ModerationCheck>>();
        protected IEnumerator<KeyValuePair<int, List<ModerationCheck>>> moderatorsKvp;

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
        public virtual void RegisterForChanges(Action<IObservableVar> callback, bool fireCallbackOnRegistration = true)
        {
            AnyChangeCallback -= callback;
            AnyChangeCallback += callback;
            anyChangeCallback_Invocations = AnyChangeCallback?.GetInvocationList();

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
            AnyChangeCallback -= callback;
            anyChangeCallback_Invocations = AnyChangeCallback?.GetInvocationList();
            this.CallbacksChanged();
        }


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
            fireElementAddedCallback_Invocations = FireElementAddedCallback?.GetInvocationList();
            this.CallbacksChanged();
        }

        public virtual void UnregisterFromElementAdded(Action<ODictionary<TKey, TValue>, TKey, TValue> callback)
        {
            FireElementAddedCallback -= callback;
            fireElementAddedCallback_Invocations = FireElementAddedCallback?.GetInvocationList();
            this.CallbacksChanged();
        }

        public virtual void RegisterForElementRemoved(Action<ODictionary<TKey, TValue>, TKey, TValue> callback)
        {
            FireElementRemovedCallback -= callback;
            FireElementRemovedCallback += callback;
            fireElementRemovedCallback_Invocations = FireElementRemovedCallback?.GetInvocationList();
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
            fireElementRemovedCallback_Invocations = FireElementRemovedCallback?.GetInvocationList();
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

                try
                {
                    this.ValueChanged();
                }
                catch(Exception e)
                {
                    Debug.LogException(new Exception("ObservableVarCallbackException", e));
                }
                
                try
                {
                    this.InvokeCallback_AnyChange();
                    this.InvokeCallback_ElementAdded(key, value);
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
                            this.InvokeCallback_AnyChange();
                            this.InvokeCallback_ElementRemoved(key, value);
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
                        this.InvokeCallback_ElementRemoved(key, itemToRemove);
                        base[key] = value;
                        this.ValueChanged();
                        this.InvokeCallback_AnyChange();
                        this.InvokeCallback_ElementAdded(key, value);
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
//* INVOCATIONS
//*====================
        private void InvokeCallback_AnyChange()
        {
            if (anyChangeCallback_Invocations != null)
            {
                Delegate[] iter_anyChangeCallback_Invocations = anyChangeCallback_Invocations;

                for (int i = iter_anyChangeCallback_Invocations.Length - 1; i >= 0 ; i--)
                {
                    Delegate invocation = iter_anyChangeCallback_Invocations[i];
                    try
                    {
                        Action<IObservableVar> action = invocation as Action<IObservableVar>;
                        action.Invoke(this);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Object obj = invocation?.Target as UnityEngine.Object;
                        Debug.LogException(e, obj);
                    }
                }
            }
        }

        private void InvokeCallback_ElementAdded(TKey key, TValue value)
        {
            if (fireElementAddedCallback_Invocations != null)
            {
                Delegate[] iter_fireElementAddedCallback_Invocations = fireElementAddedCallback_Invocations;

                for (int i = iter_fireElementAddedCallback_Invocations.Length - 1; i >= 0 ; i--)
                {
                    Delegate invocation = iter_fireElementAddedCallback_Invocations[i];
                    try
                    {
                        Action<ODictionary<TKey, TValue>, TKey, TValue> action = invocation as Action<ODictionary<TKey, TValue>, TKey, TValue>;
                        action.Invoke(this, key, value);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Object obj = invocation?.Target as UnityEngine.Object;
                        Debug.LogException(e, obj);
                    }
                }
            }
        }

        private void InvokeCallback_ElementRemoved(TKey key, TValue value)
        {
            if (fireElementRemovedCallback_Invocations != null)
            {
                Delegate[] iter_fireElementRemovedCallback_Invocations = fireElementRemovedCallback_Invocations;

                for (int i = iter_fireElementRemovedCallback_Invocations.Length - 1; i >= 0 ; i--)
                {
                    Delegate invocation = iter_fireElementRemovedCallback_Invocations[i];
                    try
                    {
                        Action<ODictionary<TKey, TValue>, TKey, TValue> action = invocation as Action<ODictionary<TKey, TValue>, TKey, TValue>;
                        action.Invoke(this, key, value);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Object obj = invocation?.Target as UnityEngine.Object;
                        Debug.LogException(e, obj);
                    }
                }
            }
        }


//*====================
//* PRIVATE
//*====================
        private bool ModerateIncomingValue(ref TKey key, ref TValue value, ODictionaryOperation operation)
        {
            while(moderatorsKvp != null && moderatorsKvp.MoveNext())
            {
                KeyValuePair<int, List<ModerationCheck>> kvp = moderatorsKvp.Current;
                List<ModerationCheck> moderatorList = kvp.Value;

                for (int i = 0; i < moderatorList.Count; i++)
                {
                    bool isAcceptable = moderatorList[i](ref key, ref value, operation);
                    if (isAcceptable == false)
                    {
                        moderatorsKvp?.Reset();
                        return false;
                    }
                }
            }
            moderatorsKvp?.Reset();
            return true;
        }

        private List<ModerationCheck> GetModerationChecks(int priority)
        {
            if (moderators.ContainsKey(priority) == false)
            {
                this.moderators.Add(priority, new List<ModerationCheck>());
                this.moderatorsKvp = this.moderators.GetEnumerator();
            }

            List<ModerationCheck> moderationChecks = this.moderators[priority];
            return moderationChecks;
        }

        public string GetCallbacks()
        {
            string callbacks = string.Empty;

            if (anyChangeCallback_Invocations != null)
            {
                foreach (Delegate invocation in anyChangeCallback_Invocations)
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

            if (fireElementAddedCallback_Invocations != null)
            {
                foreach (Delegate invocation in fireElementAddedCallback_Invocations)
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

            if (fireElementRemovedCallback_Invocations != null)
            {
                foreach (Delegate invocation in fireElementRemovedCallback_Invocations)
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

            if (anyChangeCallback_Invocations != null)
            {
                foreach (Delegate invocation in anyChangeCallback_Invocations)
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


            if (fireElementAddedCallback_Invocations != null)
            {
                foreach (Delegate invocation in fireElementAddedCallback_Invocations)
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

            if (fireElementRemovedCallback_Invocations != null)
            {
                foreach (Delegate invocation in fireElementRemovedCallback_Invocations)
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