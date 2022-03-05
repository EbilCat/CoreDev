using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CoreDev.UI
{
    public class PointerHoverAction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<bool> onPointerHover = delegate { };

        protected bool pointerEntered = false;
        protected float timeSincePointerEnter = 0f;

        public float timeToHover = 0.5f;

        void Update()
        {
            if (pointerEntered)
            {
                if (timeSincePointerEnter < timeToHover)
                {
                    timeSincePointerEnter += Time.unscaledDeltaTime;
                    if (timeSincePointerEnter >= timeToHover)
                    {
                        onPointerHover(true);
                    }
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            pointerEntered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            pointerEntered = false;
            timeSincePointerEnter = 0f;

            onPointerHover(false);
        }
    }
}
