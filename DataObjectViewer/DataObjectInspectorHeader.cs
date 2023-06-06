using UnityEngine;
using UnityEngine.EventSystems;


namespace CoreDev.DataObjectInspector
{
    public class DataObjectInspectorHeader : MonoBehaviour, IInitializePotentialDragHandler, IDragHandler
    {
        [SerializeField] private RectTransform dataObjectInspectorCanvas;
        [SerializeField] private RectTransform dataObjectInspectorWindow;
        private Vector2 lastFrameDragPos_Canvas;

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.dataObjectInspectorCanvas, eventData.position, null, out lastFrameDragPos_Canvas);
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.dataObjectInspectorCanvas, eventData.position, null, out Vector2 currentFrameDragPos_Canvas);
            Vector2 moveDelta_Canvas = currentFrameDragPos_Canvas - this.lastFrameDragPos_Canvas;
            Vector2 newPos = (Vector2)dataObjectInspectorWindow.anchoredPosition + moveDelta_Canvas;
            newPos = ClampToScreen(newPos);
            dataObjectInspectorWindow.anchoredPosition = newPos;
            lastFrameDragPos_Canvas = currentFrameDragPos_Canvas;
        }

        private Vector2 ClampToScreen(Vector2 newPos)
        {
            float width = this.dataObjectInspectorCanvas.rect.width - this.dataObjectInspectorWindow.rect.width;
            float height = this.dataObjectInspectorCanvas.rect.height - this.dataObjectInspectorWindow.rect.height;

            newPos.x = Mathf.Clamp(newPos.x, 0, width);
            newPos.y = Mathf.Clamp(newPos.y, -height, 0);

            return newPos;
        }
    }
}