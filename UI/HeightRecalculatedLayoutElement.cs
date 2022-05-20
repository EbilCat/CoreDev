using System.Collections;
using System.Collections.Generic;
using CoreDev.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class HeightRecalculatedLayoutElement : LayoutElement
{
    [SerializeField] public RectTransform heightSource = null;


    protected virtual void Update()
    {
        if (heightSource != null && heightSource.hasChanged)
        {
            UpdatePreferredHeight();
        }
    }

    protected virtual void UpdatePreferredHeight()
    {
        if (heightSource != null && ignoreLayout == false)
        {
            float newHeight = heightSource.rect.size.y;
            if (preferredHeight.AlmostEquals(newHeight) == false)
            {
                preferredHeight = newHeight;
                SetDirty();
            }
        }
    }


    //*===========================
    //* Overrides
    //*===========================
    protected override void OnEnable()
    {
        base.OnEnable();
        UpdatePreferredHeight();
        SetDirty();
    }

    protected override void OnTransformParentChanged()
    {
        UpdatePreferredHeight();
        SetDirty();
    }

    protected override void OnDisable()
    {
        UpdatePreferredHeight();
        SetDirty();
        base.OnDisable();
    }

    protected override void OnDidApplyAnimationProperties()
    {
        UpdatePreferredHeight();
        SetDirty();
    }

    protected override void OnBeforeTransformParentChanged()
    {
        UpdatePreferredHeight();
        SetDirty();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        UpdatePreferredHeight();
        SetDirty();
    }

#endif
}
