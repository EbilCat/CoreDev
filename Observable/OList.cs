using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CoreDev.Extensions;
using CoreDev.Framework;
using UnityEngine;

namespace CoreDev.Observable
{
    public enum OListOperation { ADD, REMOVE };

    public class OList<T> : List<T>, IObservableVar
    {
        public delegate bool ModerationCheck(ref T incomingValue, OListOperation op);

        protected event Action<string> ValueChangeBlocked = delegate { }; //This exists only for benefit of InspectedObservableVar
        protected event Action ValueChanged = delegate { }; //This exists only for benefit of InspectedObservableVar
        protected event Action ModeratorsChanged = delegate { }; //This exists only for benefit of InspectedObservableVar
        protected event Action CallbacksChanged = delegate { }; //This exists only for benefit of InspectedObservableVar

        private event Action<IObservableVar> AnyChangeCallback;
        private event Action<OList<T>, T> FireElementAddedCallback;
        private event Action<OList<T>, T> FireElementRemovedCallback;
        private event Action<OList<T>> FireElementsReorderedCallback;
        private Delegate[] anyChangeCallback_Invocations = null;
        private Delegate[] fireElementAddedCallback_Invocations = null;
        private Delegate[] fireElementRemovedCallback_Invocations = null;
        private Delegate[] fireElementsReorderedCallback_Invocations = null;

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

        public OList(IDataObject dataObject = null) : base()
        {
            this.DataObject = dataObject;
        }

        public OList(IEnumerable<T> collection, IDataObject dataObject = null) : base(collection)
        {
            this.DataObject = dataObject;
        }

        public OList(int capacity, IDataObject dataObject = null) : base(capacity)
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
//*==================== 027
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

        public virtual void RegisterForElementAdded(Action<OList<T>, T> callback, bool fireCallbackForExistingElements = true)
        {
            if (fireCallbackForExistingElements)
            {
                foreach (T element in this)
                {
                    callback(this, element);
                }
            }
            FireElementAddedCallback -= callback;
            FireElementAddedCallback += callback;
            fireElementAddedCallback_Invocations = FireElementAddedCallback?.GetInvocationList();
            this.CallbacksChanged();
        }

        public virtual void UnregisterFromElementAdded(Action<OList<T>, T> callback)
        {
            FireElementAddedCallback -= callback;
            fireElementAddedCallback_Invocations = FireElementAddedCallback?.GetInvocationList();
            this.CallbacksChanged();
        }

        public virtual void RegisterForElementRemoved(Action<OList<T>, T> callback)
        {
            FireElementRemovedCallback -= callback;
            FireElementRemovedCallback += callback;
            fireElementRemovedCallback_Invocations = FireElementRemovedCallback?.GetInvocationList();
            this.CallbacksChanged();
        }

        public virtual void UnregisterFromElementRemoved(Action<OList<T>, T> callback, bool fireCallbackForExistingElements = true)
        {
            if (fireCallbackForExistingElements)
            {
                foreach (T element in this)
                {
                    callback(this, element);
                }
            }
            FireElementRemovedCallback -= callback;
            fireElementRemovedCallback_Invocations = FireElementRemovedCallback?.GetInvocationList();
            this.CallbacksChanged();
        }

        public virtual void RegisterForElementsReordered(Action<OList<T>> callback)
        {
            FireElementsReorderedCallback -= callback;
            FireElementsReorderedCallback += callback;
            fireElementsReorderedCallback_Invocations = FireElementsReorderedCallback?.GetInvocationList();
            this.CallbacksChanged();
        }

        public virtual void UnregisterFromElementsReordered(Action<OList<T>> callback)
        {
            FireElementsReorderedCallback -= callback;
            fireElementsReorderedCallback_Invocations = FireElementsReorderedCallback?.GetInvocationList();
            this.CallbacksChanged();
        }


//*====================
//* ACTIONS
//*====================
        public new bool Add(T item)
        {
            bool moderationPassed = this.ModerateIncomingValue(ref item, OListOperation.ADD);
            if (moderationPassed)
            {
                base.Add(item);
                this.ValueChanged();
                try
                {
                    this.InvokeCallback_AnyChange();
                    this.InvokeCallback_ElementAdded(item);
                    this.InvokeCallback_ElementsReordered();
                }
                catch (Exception e)
                {
                    Debug.LogException(new Exception("ObservableVarCallbackException", e));
                }
            }
            return moderationPassed;
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                this.Add(item);
            }
        }

        public new bool Insert(int index, T item)
        {
            bool moderationPassed = this.ModerateIncomingValue(ref item, OListOperation.ADD);
            if (moderationPassed)
            {
                base.Insert(index, item);
                this.ValueChanged();
                try
                {
                    this.InvokeCallback_AnyChange();
                    this.InvokeCallback_ElementAdded(item);
                    this.InvokeCallback_ElementsReordered();
                }
                catch (Exception e)
                {
                    Debug.LogException(new Exception("ObservableVarCallbackException", e));
                }
            }
            return moderationPassed;
        }

        public new void InsertRange(int index, IEnumerable<T> enumerable)
        {
            foreach (T item in enumerable)
            {
                bool succes = this.Insert(index, item);
                if (succes)
                {
                    index++;
                }
            }
        }

        public new bool Remove(T item)
        {
            bool isRemoved = false;

            bool moderationPassed = this.ModerateIncomingValue(ref item, OListOperation.REMOVE);
            if (moderationPassed)
            {
                isRemoved = base.Remove(item);

                if (isRemoved)
                {
                    this.ValueChanged();
                    try
                    {
                        this.InvokeCallback_AnyChange();
                        this.InvokeCallback_ElementRemoved(item);
                        this.InvokeCallback_ElementsReordered();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(new Exception("ObservableVarCallbackException", e));
                    }
                }
            }

            return isRemoved;
        }

        public new int RemoveAll(Predicate<T> match)
        {
            List<T> collection = this.FindAll(match);

            int removalCount = 0;

            foreach (T item in collection)
            {
                if (this.Remove(item))
                {
                    removalCount++;
                }
            }

            return removalCount;
        }

        public new bool RemoveAt(int index)
        {
            T item = this[index];

            bool moderationPassed = this.ModerateIncomingValue(ref item, OListOperation.REMOVE);
            if (moderationPassed)
            {
                base.RemoveAt(index);
                this.ValueChanged();
                try
                {
                    this.InvokeCallback_AnyChange();
                    this.InvokeCallback_ElementRemoved(item);
                    this.InvokeCallback_ElementsReordered();
                }
                catch (Exception e)
                {
                    Debug.LogException(new Exception("ObservableVarCallbackException", e));
                }
            }
            return moderationPassed;
        }

        public new void RemoveRange(int index, int count)
        {
            if (index >= 0 && index < this.Count && count <= this.Count)
            {
                for (int i = 0; i < count; i++)
                {
                    this.RemoveAt(index);
                }
            }
        }

        public new void Reverse(int index, int count)
        {
            base.Reverse(index, count);
            this.ValueChanged();
            try
            {
                this.InvokeCallback_AnyChange();
                this.InvokeCallback_ElementsReordered();
            }
            catch (Exception e)
            {
                Debug.LogException(new Exception("ObservableVarCallbackException", e));
            }
        }

        public new void Reverse()
        {
            base.Reverse();
            this.ValueChanged();
            try
            {
                this.InvokeCallback_AnyChange();
                this.InvokeCallback_ElementsReordered();
            }
            catch (Exception e)
            {
                Debug.LogException(new Exception("ObservableVarCallbackException", e));
            }
        }

        public new void Sort(Comparison<T> comparison)
        {
            base.Sort(comparison);
            this.ValueChanged();
            try
            {
                this.InvokeCallback_AnyChange();
                this.InvokeCallback_ElementsReordered();
            }
            catch (Exception e)
            {
                Debug.LogException(new Exception("ObservableVarCallbackException", e));
            }
        }

        public new void Sort(int index, int count, IComparer<T> comparer)
        {
            base.Sort(index, count, comparer);
            this.ValueChanged();
            try
            {
                this.InvokeCallback_AnyChange();
                this.InvokeCallback_ElementsReordered();
            }
            catch (Exception e)
            {
                Debug.LogException(new Exception("ObservableVarCallbackException", e));
            }
        }

        public new void Sort()
        {
            base.Sort();
            this.ValueChanged();
            this.InvokeCallback_AnyChange();
            this.InvokeCallback_ElementsReordered();
        }

        public new void Sort(IComparer<T> comparer)
        {
            base.Sort(comparer);
            this.ValueChanged();
            try
            {
                this.InvokeCallback_AnyChange();
                this.InvokeCallback_ElementsReordered();
            }
            catch (Exception e)
            {
                Debug.LogException(new Exception("ObservableVarCallbackException", e));
            }
        }

        public new void Clear()
        {
            int elementCount = this.Count;
            for (int i = 0; i < elementCount; i++)
            {
                this.RemoveAt(0);
            }
        }


//*====================
//* QUERIES
//*====================
        public new T this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                T itemToRemove = base[index];
                bool removalModerationPassed = this.ModerateIncomingValue(ref itemToRemove, OListOperation.REMOVE);
                bool additionModerationPassed = this.ModerateIncomingValue(ref value, OListOperation.ADD);

                if (removalModerationPassed && additionModerationPassed)
                {
                    try
                    {
                        this.InvokeCallback_ElementRemoved(itemToRemove);
                        base[index] = value;
                        this.ValueChanged();
                        this.InvokeCallback_AnyChange();
                        this.InvokeCallback_ElementAdded(value);
                        this.InvokeCallback_ElementsReordered();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(new Exception("ObservableVarCallbackException", e));
                    }
                }
            }
        }

        public ReadOnlyCollection<T> Value
        {
            get
            {
                return base.AsReadOnly();
            }
        }

        public override string ToString()
        {
            return this.Value.Count.ToString();
        }


//*====================
//* INVOCATIONS
//*====================
        private void InvokeCallback_AnyChange()
        {
            if (anyChangeCallback_Invocations != null)
            {
                Delegate[] iter_anyChangeCallback_Invocations = anyChangeCallback_Invocations;

                for (int i = 0; i < iter_anyChangeCallback_Invocations.Length; i++)
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

        private void InvokeCallback_ElementAdded(T element)
        {
            if (fireElementAddedCallback_Invocations != null)
            {
                Delegate[] iter_fireElementAddedCallback_Invocations = fireElementAddedCallback_Invocations;

                for (int i = 0; i < iter_fireElementAddedCallback_Invocations.Length; i++)
                {
                    Delegate invocation = iter_fireElementAddedCallback_Invocations[i];
                    try
                    {
                        Action<OList<T>, T> action = invocation as Action<OList<T>, T>;
                        action.Invoke(this, element);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Object obj = invocation?.Target as UnityEngine.Object;
                        Debug.LogException(e, obj);
                    }
                }
            }
        }

        private void InvokeCallback_ElementRemoved(T element)
        {
            if (fireElementRemovedCallback_Invocations != null)
            {
                Delegate[] iter_fireElementRemovedCallback_Invocations = fireElementRemovedCallback_Invocations;

                for (int i = 0; i < iter_fireElementRemovedCallback_Invocations.Length; i++)
                {
                    Delegate invocation = iter_fireElementRemovedCallback_Invocations[i];
                    try
                    {
                        Action<OList<T>, T> action = invocation as Action<OList<T>, T>;
                        action.Invoke(this, element);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Object obj = invocation?.Target as UnityEngine.Object;
                        Debug.LogException(e, obj);
                    }
                }
            }
        }

        private void InvokeCallback_ElementsReordered()
        {
            if (fireElementsReorderedCallback_Invocations != null)
            {
                Delegate[] iter_fireElementsReorderedCallback_Invocations = fireElementsReorderedCallback_Invocations;
                for (int i = 0; i < iter_fireElementsReorderedCallback_Invocations.Length; i++)
                {
                    Delegate invocation = iter_fireElementsReorderedCallback_Invocations[i];
                    try
                    {
                        Action<OList<T>> action = invocation as Action<OList<T>>;
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


//*====================
//* PRIVATE
//*====================
        private bool ModerateIncomingValue(ref T value, OListOperation operation)
        {
            while (moderatorsKvp != null && moderatorsKvp.MoveNext())
            {
                KeyValuePair<int, List<ModerationCheck>> kvp = moderatorsKvp.Current;
                List<ModerationCheck> moderatorList = kvp.Value;

                for (int i = 0; i < moderatorList.Count; i++)
                {
                    bool isAcceptable = moderatorList[i](ref value, operation);
                    if (isAcceptable == false)
                    {
                        this.ValueChangeBlocked(moderatorList[i].Method.Name);
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
                        callbacks += $"ADD: {invocation.Target.GetType().Name} ({monoBehaviour.name})\r\n";
                    }
                    else
                    {
                        callbacks += $"ADD: {invocation.Target.GetType().Name}\r\n";
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
                        callbacks += $"REMOVE: {invocation.Target.GetType().Name} ({monoBehaviour.name})\r\n";
                    }
                    else
                    {
                        callbacks += $"REMOVE: {invocation.Target.GetType().Name}\r\n";
                    }
                }
            }

            if (fireElementsReorderedCallback_Invocations != null)
            {
                foreach (Delegate invocation in fireElementsReorderedCallback_Invocations)
                {
                    if (invocation.Target is MonoBehaviour)
                    {
                        MonoBehaviour monoBehaviour = invocation.Target as MonoBehaviour;
                        callbacks += $"REORDERED: {invocation.Target.GetType().Name} ({monoBehaviour.name})\r\n";
                    }
                    else
                    {
                        callbacks += $"REORDERED: {invocation.Target.GetType().Name}\r\n";
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
                        callbacks.Add($"ADD: {invocation.Target.GetType().Name} ({monoBehaviour.name})");
                    }
                    else
                    {
                        callbacks.Add($"ADD: {invocation.Target.GetType().Name}");
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
                        callbacks.Add($"REMOVE: {invocation.Target.GetType().Name} ({monoBehaviour.name})");
                    }
                    else
                    {
                        callbacks.Add($"REMOVE: {invocation.Target.GetType().Name}");
                    }
                }
            }

            if (fireElementsReorderedCallback_Invocations != null)
            {
                foreach (Delegate invocation in fireElementsReorderedCallback_Invocations)
                {
                    if (invocation.Target is MonoBehaviour)
                    {
                        MonoBehaviour monoBehaviour = invocation.Target as MonoBehaviour;
                        callbacks.Add($"REORDERED: {invocation.Target.GetType().Name} ({monoBehaviour.name})");
                    }
                    else
                    {
                        callbacks.Add($"REORDERED: {invocation.Target.GetType().Name}");
                    }
                }
            }
        }

        public virtual void SetValueFromString(string strVal)
        {
            Debug.LogWarning($"No override for SetValueFromString for type {this.GetType()}");
            if (strVal.ToLower().Contains("clear") || strVal.ToLower().Contains("clr"))
            {
                this.Clear();
            }
        }

        public virtual byte[] ToBytes()
        {
            T[] array = this.ToArray();
            byte[] bytes = SerializationHelper.Serialize(array);
            return bytes;
        }

        public virtual void SetValueFromBytes(byte[] bytes)
        {
            T[] array = SerializationHelper.Deserialize<T[]>(bytes);
            this.Clear();
            this.AddRange(array);
        }
    }
}