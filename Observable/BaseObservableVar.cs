using System;
using System.Collections.Generic;
using CoreDev.Framework;
using UnityEngine;

namespace CoreDev.Observable
{
    public abstract class ObservableVar<T> : IObservableVar
    {
        public delegate bool ModerationCheck(ref T incomingValue);
        protected event Action ValueChanged = delegate { };
        private event Action<ObservableVar<T>> FireCallbacks = delegate { };
        protected event Action ModeratorsChanged = delegate { };

        [SerializeField] protected T currentValue;
        protected T previousValue;
        protected SortedList<int, List<ModerationCheck>> moderators = new SortedList<int, List<ModerationCheck>>();
        protected bool isValueLocked = false;

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
                if (isValueLocked) //Prevent recursion. None of the observers should be allowed to change value while ValueChanged is fired
                {
                    return;
                }

                this.isValueLocked = true;

                T moderatedValue = value;
                bool isValueAcceptable = ModerateIncomingValue(ref moderatedValue);

                if (isValueAcceptable)
                {
                    if (AreEqual(currentValue, moderatedValue) == false)
                    {
                        previousValue = currentValue;
                        currentValue = moderatedValue;
                        ValueChanged();
                    }
                }

                this.isValueLocked = false;
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

            this.ValueChanged += () => { this.FireCallbacks(this); };
        }


//*====================
//* REGISTRATION FOR CHANGES
//*====================
        public void RegisterForChanges(Action<ObservableVar<T>> callback, bool fireCallbackOnRegistration = true)
        {
            FireCallbacks -= callback;
            FireCallbacks += callback;
            if (fireCallbackOnRegistration) { callback(this); }
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
            return string.Format("ObservableVar<{0}>({1})", typeof(T).Name, Value.ToString());
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
    }
}