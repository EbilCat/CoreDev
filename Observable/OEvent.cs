
using System;
using System.Collections.Generic;
using CoreDev.Framework;
using UnityEngine;

namespace CoreDev.Observable
{
    public class OEvent : IObservableVar
    {
        protected event Action<string> ValueChangeBlocked = delegate { };  //This exists only for benefit of InspectedObservableEvent
        protected event Action ValueChanged = delegate { }; //This exists only for benefit of InspectedObservableEvent
        protected event Action ModeratorsChanged = delegate { }; //This exists only for benefit of InspectedObservableEvent
        protected event Action CallbacksChanged = delegate { }; //This exists only for benefit of InspectedObservableEvent

        private const bool warnOnRecursion = true;
        public delegate bool ModerationCheck();
        private event Action FireCallbacks;
        private Delegate[] invocationList;

        protected SortedList<int, List<ModerationCheck>> moderators = new SortedList<int, List<ModerationCheck>>();
        protected IEnumerator<KeyValuePair<int, List<ModerationCheck>>> moderatorEnumerator;
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

//*====================
//* CONSTRUCTORS
//*====================
        public OEvent() : this(default(IDataObject)) { }

        public OEvent(IDataObject dataObject)
        {
            this.dataObject = dataObject;
        }


//*====================
//* REGISTRATION FOR CHANGES
//*====================
        public virtual void RegisterForChanges(Action callback)
        {
            FireCallbacks -= callback;
            FireCallbacks += callback;
            this.invocationList = FireCallbacks?.GetInvocationList();
            this.CallbacksChanged();
        }

        public void UnregisterFromChanges(Action callback)
        {
            FireCallbacks -= callback;
            this.invocationList = FireCallbacks?.GetInvocationList();

            this.CallbacksChanged();
        }


//*====================
//* MODERATORS
//*====================
        public void AddModerator(ModerationCheck acceptanceCheck, int priority = 0, bool applyModeratorImmediately = true)
        {
            List<ModerationCheck> moderationChecks = GetModerationChecks(priority);

            if (moderationChecks.Contains(acceptanceCheck) == false)
            {
                moderationChecks.Add(acceptanceCheck);
                this.ModeratorsChanged();
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


//*====================
//* PUBLIC
//*====================
        public void Fire()
        {
            if (recursionCount >= recursionLimit)
            {
                throw new InfiniteRecursionException($"Possible infinite recursion. This variable has been edited {recursionCount} in a single call. \r\nIf this was intentional, please raise the value of \"RecursionLimit\" in BaseObservableVar.cs to avoid this Exception\r\n");
            }

            this.recursionCount++;

            bool allModeratorsPassed = Moderate();

            if (allModeratorsPassed)
            {
                this.ValueChanged();

                if (invocationList != null)
                {
                    foreach (Delegate invocation in invocationList)
                    {
                        Action action = invocation as Action;
                        try
                        {
                            action.Invoke();
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(new Exception("ObservableVarCallbackException", e));
                        }
                    }
                }
            }

            this.recursionCount = 0;
        }


//*====================
//* UTILS
//*====================
        private bool Moderate()
        {
            while (moderatorEnumerator != null && moderatorEnumerator.MoveNext())
            {
                KeyValuePair<int, List<ModerationCheck>> kvp = moderatorEnumerator.Current;
                List<ModerationCheck> moderatorList = kvp.Value;

                for (int i = 0; i < moderatorList.Count; i++)
                {
                    bool isAcceptable = moderatorList[i]();
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

        public string GetCallbacks()
        {
            string callbacks = string.Empty;

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

        public void GetCallbacks(List<string> callbacks)
        {
            callbacks.Clear();

            if (invocationList != null)
            {
                for (int i = 0; i < invocationList.Length; i++)
                {
                    Delegate invocation = invocationList[i];

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

        public void SetValueFromString(string strVal) { }
    }
}