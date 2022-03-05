using System;
using System.Collections;
using System.Collections.Generic;
using CoreDev.Observable;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public abstract class ObserverText<T> : MonoBehaviour
{
    protected Text text;
    protected ObservableVar<T> cachedVar = null;

    protected virtual void Awake()
    {
        text = this.GetComponent<Text>();
        text.text = string.Empty;
    }

    protected virtual void OnDestroy()
    {
        ResetCache();
    }

    protected virtual void Init(ObservableVar<T> var)
    {
        if (var != null)
        {
            ResetCache();
            cachedVar = var;
            var.RegisterForChanges(UpdateText);
        }
    }

    protected virtual void ResetCache()
    {
        if (cachedVar != null)
        {
            cachedVar.UnregisterFromChanges(UpdateText);
            cachedVar = null;
        }
    }

    protected virtual void UpdateText(ObservableVar<T> value)
    {
        text.text = GetText(value);
    }

    protected abstract string GetText(ObservableVar<T> value);
}
