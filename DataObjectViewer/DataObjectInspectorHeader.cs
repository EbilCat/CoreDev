using UnityEngine;
using UnityEngine.EventSystems;


namespace CoreDev.DataObjectInspector
{
    public class DataObjectInspectorHeader : MonoBehaviour, IInitializePotentialDragHandler, IDragHandler
    {
        private Vector2 leftDragInitPos_Screen = Vector2.zero;
        private Vector2 posDragInitPos_Screen = Vector2.zero;
        private Vector2 rightDragInitPos_Screen = Vector2.zero;
        private Vector2 resizeInitSize = Vector2.zero;
        private RectTransform windowRecTransform;


//*====================
//* UNITY
//*====================
        private void Awake()
        {
            this.windowRecTransform = this.transform.parent.GetComponent<DataObjectInspectorWindow>().transform as RectTransform;
        }


//*====================
//* POINTER HANDLERS
//*====================
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            PointerEventData.InputButton button = eventData.button;

            if (button == PointerEventData.InputButton.Left)
            {
                leftDragInitPos_Screen = eventData.position;
                posDragInitPos_Screen = this.windowRecTransform.anchoredPosition;
            }
            else
            if (button == PointerEventData.InputButton.Right)
            {
                rightDragInitPos_Screen = eventData.position;
                resizeInitSize = this.windowRecTransform.sizeDelta;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            PointerEventData.InputButton button = eventData.button;
            if (button == PointerEventData.InputButton.Left)
            {
                this.windowRecTransform.anchoredPosition = posDragInitPos_Screen + (eventData.position - leftDragInitPos_Screen);
            }
            else
            if (button == PointerEventData.InputButton.Right)
            {
                Vector2 pointerPosDelta_Screen = (eventData.position - rightDragInitPos_Screen);
                pointerPosDelta_Screen.y *= -1;
                this.windowRecTransform.sizeDelta = resizeInitSize + pointerPosDelta_Screen;
            }
        }
    }
}