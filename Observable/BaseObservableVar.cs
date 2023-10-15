using System;
using System.Collections.Generic;
using CoreDev.Extensions;
using CoreDev.Framework;
using UnityEngine;

namespace CoreDev.Observable
{
    public class InfiniteRecursionException : Exception
    {
        public InfiniteRecursionException(string message) : base(message)
        {
        }
    }

    public abstract class ObservableVar<T> : IObservableVar
    {
        protected event Action<string> ValueChangeBlocked = delegate { };  //This exists only for benefit of InspectedObservableVar
        protected event Action ValueChanged = delegate { }; //This exists only for benefit of InspectedObservableVar
        protected event Action ModeratorsChanged = delegate { }; //This exists only for benefit of InspectedObservableVar
        protected event Action CallbacksChanged = delegate { }; //This exists only for benefit of InspectedObservableVar

        private const bool warnOnRecursion = true;
        private bool equalityChecksEnabled = true;
        public bool EqualityChecksEnabled => equalityChecksEnabled;
        public delegate bool ModerationCheck(ref T incomingValue);
        private event Action<ObservableVar<T>> Callbacks_Derived;
        private event Action<IObservableVar> Callbacks_Interface;
        private Delegate[] callbacks_Derived_Invocations = null;
        private Delegate[] callbacks_Interface_Invocations = null;

        [SerializeField] protected T currentValue;
        protected T previousValue;
        protected SortedList<int, List<ModerationCheck>> moderators = new SortedList<int, List<ModerationCheck>>();
        protected IEnumerator<KeyValuePair<int, List<ModerationCheck>>> moderatorEnumerator;
        protected byte recursionCount = 0;
        protected const byte recursionLimit = 100;

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

        public T PreviousValue
        {
            get { return previousValue; }
        }

        public virtual T Value
        {
            get { return currentValue; }
            set
            {
                this.FireCallbacks_Moderated(value);
            }
        }


        //*====================
        //* CONSTRUCTORS
        //*====================
        public ObservableVar() : this(default(T)) { }

        public ObservableVar(T startValue) : this(startValue, default(IDataObject)) { }

        public ObservableVar(T startValue, IDataObject dataObject)
        {
            this.dataObject = dataObject;

            this.previousValue = startValue;
            this.currentValue = startValue;
        }


        //*====================
        //* PUBLIC
        //*====================
        public virtual void RegisterForChanges(Action<ObservableVar<T>> callback, bool fireCallbackOnRegistration = true)
        {
            Callbacks_Derived -= callback;
            Callbacks_Derived += callback;
            this.callbacks_Derived_Invocations = Callbacks_Derived?.GetInvocationList();

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

        public virtual void UnregisterFromChanges(Action<ObservableVar<T>> callback)
        {
            Callbacks_Derived -= callback;
            this.callbacks_Derived_Invocations = Callbacks_Derived?.GetInvocationList();
            this.CallbacksChanged();
        }

        public virtual void RegisterForChanges(Action<IObservableVar> callback, bool fireCallbackOnRegistration = true)
        {
            Callbacks_Interface -= callback;
            Callbacks_Interface += callback;
            this.callbacks_Interface_Invocations = Callbacks_Interface?.GetInvocationList();

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
            Callbacks_Interface -= callback;
            this.callbacks_Interface_Invocations = Callbacks_Interface?.GetInvocationList();
            this.CallbacksChanged();
        }


        public void AddModerator(ModerationCheck acceptanceCheck, int priority = 0, bool applyModeratorImmediately = true)
        {
            List<ModerationCheck> moderationChecks = GetModerationChecks(priority);

            if (moderationChecks.Contains(acceptanceCheck) == false)
            {
                moderationChecks.Add(acceptanceCheck);
                this.ModeratorsChanged();

                if (applyModeratorImmediately)
                {
                    this.Value = this.currentValue;
                }
            }
        }

        public void RemoveModerator(ModerationCheck acceptanceCheck, int priority = 0)
        {
            List<ModerationCheck> moderationChecks = GetModerationChecks(priority);
            bool moderatorRemoved = moderationChecks.Remove(acceptanceCheck);
            if (moderatorRemoved)
            {
                this.ModeratorsChanged();
            }
        }

        public void EnableEqualityChecks(bool enable)
        {
            this.equalityChecksEnabled = enable;
        }

        public bool IsEquals(ObservableVar<T> other)
        {
            return AreEqual(currentValue, other.Value);
        }

        public bool IsValueEquals(T other)
        {
            return AreEqual(currentValue, other);
        }

        public override string ToString()
        {
            if (Value == null)
            {
                return "<NULL>";
            }
            else
            {
                return Value.ToString();
            }
        }

        public virtual void SetValueFromString(string strVal)
        {
            Debug.LogWarning($"No override for SetValueFromString for type {this.GetType()}");
        }

        public virtual byte[] ToBytes()
        {
            byte[] bytes = SerializationHelper.Serialize(this.Value);
            if (bytes == null)
            {
                Debug.Log(this.GetType().ToString());
                bytes = new byte[0];
            }
            return bytes;
        }

        public virtual void SetValueFromBytes(byte[] bytes)
        {
            this.Value = SerializationHelper.Deserialize<T>(bytes);
        }

        public string GetCallbacks()
        {
            string callbackStr = string.Empty;
            callbackStr = AppendToCallbackString(callbackStr, callbacks_Derived_Invocations);
            callbackStr = AppendToCallbackString(callbackStr, callbacks_Interface_Invocations);
            return callbackStr;
        }

        public void GetCallbacks(List<string> callbacks)
        {
            callbacks.Clear();
            AppendToCallbackList(callbacks, callbacks_Derived_Invocations);
            AppendToCallbackList(callbacks, callbacks_Interface_Invocations);
        }


        //*====================
        //* PRIVATE
        //*====================
        protected void FireCallbacks_Moderated(T value)
        {
            if (recursionCount >= recursionLimit)
            {
                throw new InfiniteRecursionException($"Possible infinite recursion. This variable has been edited {recursionCount} in a single call. \r\nIf this was intentional, please raise the value of \"RecursionLimit\" in BaseObservableVar.cs to avoid this Exception\r\n");
            }

            this.recursionCount++;

            T moderatedValue = value;
            bool isValueAcceptable = ModerateIncomingValue(ref moderatedValue);

            if (isValueAcceptable)
            {
                if (equalityChecksEnabled == false || AreEqual(currentValue, moderatedValue) == false)
                {
                    previousValue = currentValue;
                    currentValue = moderatedValue;
                    FireCallbacks();
                }
            }

            this.recursionCount = 0;
        }

        protected void FireCallbacks()
        {
            this.ValueChanged();

            if (callbacks_Derived_Invocations != null)
            {
                Delegate[] iter_callbacks_Derived_Invocations = callbacks_Derived_Invocations;

                for (int i = 0; i < iter_callbacks_Derived_Invocations.Length; i++)
                {
                    Delegate invocation = iter_callbacks_Derived_Invocations[i];
                    try
                    {
                        Action<ObservableVar<T>> action = (Action<ObservableVar<T>>)invocation;
                        action.Invoke(this);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Object obj = invocation?.Target as UnityEngine.Object;
                        Debug.LogException(e, obj);
                    }
                }
            }

            if (callbacks_Interface_Invocations != null)
            {
                Delegate[] iter_callbacks_Interface_Invocations = callbacks_Interface_Invocations;

                for (int i = 0; i < iter_callbacks_Interface_Invocations.Length; i++)
                {
                    Delegate invocation = iter_callbacks_Interface_Invocations[i];
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

        protected bool ModerateIncomingValue(ref T value)
        {
            while (moderatorEnumerator != null && moderatorEnumerator.MoveNext())
            {
                KeyValuePair<int, List<ModerationCheck>> kvp = moderatorEnumerator.Current;
                List<ModerationCheck> moderatorList = kvp.Value;

                for (int i = 0; i < moderatorList.Count; i++)
                {
                    bool isAcceptable = moderatorList[i](ref value);
                    if (isAcceptable == false)
                    {
                        this.ValueChangeBlocked(moderatorList[i].Method.Name);
                        moderatorEnumerator?.Reset();
                        return false;
                    }
                }
            }

            moderatorEnumerator?.Reset();
            return true;
        }

        private List<ModerationCheck> GetModerationChecks(int priority)
        {
            if (moderators.ContainsKey(priority) == false)
            {
                this.moderators.Add(priority, new List<ModerationCheck>());
                this.moderatorEnumerator = moderators.GetEnumerator();
            }

            List<ModerationCheck> moderationChecks = this.moderators[priority];
            return moderationChecks;
        }

        protected abstract bool AreEqual(T var, T value);

        protected virtual T ModerateValue(T input)
        {
            return input;
        }

        private static string AppendToCallbackString(string callbacks, Delegate[] invocationList)
        {
            if (invocationList != null)
            {
                for (int i = 0; i < invocationList.Length; i++)
                {
                    Delegate invocation = invocationList[i];

                    if (invocation.Target is MonoBehaviour)
                    {
                        MonoBehaviour monoBehaviour = invocation.Target as MonoBehaviour;
                        callbacks += $"{invocation.Target.GetType().Name} ({monoBehaviour.name})";
                    }
                    else
                    {
                        callbacks += $"{invocation.Target.GetType().Name}";
                    }

                    if (i < invocationList.Length - 1)
                    {
                        callbacks += "\r\n";
                    }
                }
            }

            return callbacks;
        }

        private void AppendToCallbackList(List<string> callbacks, Delegate[] delegates)
        {
            if (delegates != null)
            {
                for (int i = 0; i < delegates.Length; i++)
                {
                    Delegate invocation = delegates[i];

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
    }
}