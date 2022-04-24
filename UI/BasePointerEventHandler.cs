using CoreDev.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;


namespace CoreDev.UI
{
    public class BasePointerEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IScrollHandler
    {
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            this.gameObject.BubbleEvent(eventData, ExecuteEvents.beginDragHandler);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            this.gameObject.BubbleEvent(eventData, ExecuteEvents.dragHandler);
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
            this.gameObject.BubbleEvent(eventData, ExecuteEvents.dropHandler);
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            this.gameObject.BubbleEvent(eventData, ExecuteEvents.endDragHandler);
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            this.gameObject.BubbleEvent(eventData, ExecuteEvents.initializePotentialDrag);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            this.gameObject.BubbleEvent(eventData, ExecuteEvents.pointerClickHandler);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            this.gameObject.BubbleEvent(eventData, ExecuteEvents.pointerDownHandler);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            this.gameObject.BubbleEvent(eventData, ExecuteEvents.pointerEnterHandler);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            this.gameObject.BubbleEvent(eventData, ExecuteEvents.pointerExitHandler);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            this.gameObject.BubbleEvent(eventData, ExecuteEvents.pointerUpHandler);
        }

        public virtual void OnScroll(PointerEventData eventData)
        {
            this.gameObject.BubbleEvent(eventData, ExecuteEvents.scrollHandler);
        }
    }
}