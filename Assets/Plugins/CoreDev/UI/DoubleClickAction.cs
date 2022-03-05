using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CoreDev.UI
{
    public class DoubleClickAction : MonoBehaviour, IPointerClickHandler
    {
        public event Action onDoubleClick = delegate { };
        
        public enum DoubleClickType
        {
            LeftButton,
            RightButton,
            MiddleButton,
            AnyButton
        }
        public DoubleClickType doubleClickType = DoubleClickType.LeftButton;

        protected PointerEventData.InputButton previousClickedButton = PointerEventData.InputButton.Left;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsDoubleClick(eventData)) { onDoubleClick(); }
            previousClickedButton = eventData.button;
        }

        protected virtual bool IsDoubleClick(PointerEventData eventData)
        {
            if (eventData.clickCount > 1)
            {
                switch (doubleClickType)
                {
                    case DoubleClickType.LeftButton:
                        return eventData.button == PointerEventData.InputButton.Left && eventData.button == previousClickedButton;
                    case DoubleClickType.RightButton:
                        return eventData.button == PointerEventData.InputButton.Right && eventData.button == previousClickedButton;
                    case DoubleClickType.MiddleButton:
                        return eventData.button == PointerEventData.InputButton.Middle && eventData.button == previousClickedButton;
                    default:
                    case DoubleClickType.AnyButton:
                        return true;
                }
            }
            return false;
        }
    }
}
