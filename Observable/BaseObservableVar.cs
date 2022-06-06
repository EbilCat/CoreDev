using System;
using System.Collections.Generic;
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

        private const bool warnOnRecursion = true;
        public delegate bool ModerationCheck(ref T incomingValue);
        private event Action<ObservableVar<T>> FireCallbacks;

        [SerializeField] protected T currentValue;
        protected T previousValue;
        protected SortedList<int, List<ModerationCheck>> moderators = new SortedList<int, List<ModerationCheck>>();
        protected int recursionCount = 0;
        protected const int recursionLimit = 1000;

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

        public T Value
        {
            get { return currentValue; }
            set
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
                    if (AreEqual(currentValue, moderatedValue) == false)
                    {
                        previousValue = currentValue;
                        currentValue = moderatedValue;

                        this.ValueChanged();

                        Delegate[] invocationList = FireCallbacks?.GetInvocationList();
                        if (invocationList != null)
                        {
                            foreach (Delegate invocation in invocationList)
                            {
                                Action<ObservableVar<T>> action = invocation as Action<ObservableVar<T>>;
                                try
                                {
                                    action.Invoke(this);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogException(new Exception("ObservableVarCallbackException", e));
                                }
                            }
                        }
                    }
                }

                this.recursionCount = 0;
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
//* REGISTRATION FOR CHANGES
//*====================
        public virtual void RegisterForChanges(Action<ObservableVar<T>> callback, bool fireCallbackOnRegistration = true)
        {
            FireCallbacks -= callback;
            FireCallbacks += callback;
            if (fireCallbackOnRegistration)
            {
                try
                {
                    // Debug.Log($"{callback.Method.DeclaringType.Name}.{callback.Method.Name}");
                    callback(this);
                }
                catch (Exception e)
                {
                    Debug.LogException(new Exception("ObservableVarCallbackException", e));
                }
            }
        }

        public void UnregisterFromChanges(Action<ObservableVar<T>> callback)
        {
            FireCallbacks -= callback;
        }


//*====================
//* MODERATORS
//*====================
        public void AddModerator(ModerationCheck acceptanceCheck, int priority = 0, bool applyModeratorImmediately = true)
        {
            List<ModerationCheck> moderationChecks = GetModerationChecks(priority);
            moderationChecks.Remove(acceptanceCheck);
            moderationChecks.Add(acceptanceCheck);
            this.ModeratorsChanged();

            if (applyModeratorImmediately)
            {
                this.Value = this.currentValue;
            }
        }

        public void RemoveModerator(ModerationCheck acceptanceCheck, int priority = 0)
        {
            List<ModerationCheck> moderationChecks = GetModerationChecks(priority);
            moderationChecks.Remove(acceptanceCheck);
            this.ModeratorsChanged();
        }


//*====================
//* UTILS
//*====================
        private bool ModerateIncomingValue(ref T value)
        {
            foreach (KeyValuePair<int, List<ModerationCheck>> kvp in moderators)
            {
                List<ModerationCheck> moderatorList = kvp.Value;

                for (int i = 0; i < moderatorList.Count; i++)
                {
                    bool isAcceptable = moderatorList[i](ref value);
                    if (isAcceptable == false)
                    {
                        this.ValueChangeBlocked(moderatorList[i].Method.Name);
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

        protected abstract bool AreEqual(T var, T value);

        protected virtual T ModerateValue(T input)
        {
            return input;
        }

        public string GetCallbacks()
        {
            string callbacks = string.Empty;

            Delegate[] invocationList = FireCallbacks?.GetInvocationList();
            if (invocationList != null)
            {
                for (int i = 0; i < invocationList.Length; i++)
                {
                    Delegate invocation = invocationList[i];
                    callbacks = $"{invocation.Method.DeclaringType.Name}.{invocation.Method.Name}";
                    if (i < invocationList.Length - 1)
                    {
                        callbacks += "\r\n";
                    }
                }
            }
            return callbacks;
        }
    }
}