using UnityEngine;
using UnityEngine.EventSystems;


namespace CoreDev.DataObjectInspector
{
    public class DataObjectInspectorResizeHandle : MonoBehaviour, IInitializePotentialDragHandler, IDragHandler
    {
        [SerializeField] private RectTransform dataObjectInspectorCanvas;
        [SerializeField] private RectTransform dataObjectInspectorWindow;
        [SerializeField] private float minSize = 30.0f;
        private Vector2 lastFrameDragPos_Canvas;

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.dataObjectInspectorCanvas, eventData.position, null, out lastFrameDragPos_Canvas);
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.dataObjectInspectorCanvas, eventData.position, null, out Vector2 currentFrameDragPos_Canvas);
            Vector2 currentSizeDelta = dataObjectInspectorWindow.sizeDelta;
            Vector2 sizeDelta = currentFrameDragPos_Canvas - this.lastFrameDragPos_Canvas;
            sizeDelta.y *= -1; //Inverse
            Vector2 newSizeDelta = currentSizeDelta + sizeDelta;
            newSizeDelta = ClampToScreen(newSizeDelta);
            dataObjectInspectorWindow.sizeDelta = newSizeDelta;
            lastFrameDragPos_Canvas = currentFrameDragPos_Canvas;
        }

        private Vector2 ClampToScreen(Vector2 newSizeDelta)
        {
            float width = this.dataObjectInspectorCanvas.rect.width;
            float height = this.dataObjectInspectorCanvas.rect.height;

            newSizeDelta.x = Mathf.Clamp(newSizeDelta.x, this.minSize, width - this.dataObjectInspectorWindow.anchoredPosition.x);
            newSizeDelta.y = Mathf.Clamp(newSizeDelta.y, this.minSize, height - this.dataObjectInspectorWindow.anchoredPosition.y);

            return newSizeDelta;
        }
    }
}