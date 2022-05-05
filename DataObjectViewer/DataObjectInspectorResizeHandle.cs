using UnityEngine;
using UnityEngine.EventSystems;


namespace CoreDev.DataObjectInspector
{
    public class DataObjectInspectorResizeHandle : MonoBehaviour, IInitializePotentialDragHandler, IDragHandler
    {
        [SerializeField] private RectTransform dataObjectInspectorWindow;
        [SerializeField] private float minSize = 30.0f;
        private Vector2 lastFrameDragPos_Screen;

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            lastFrameDragPos_Screen = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 currentSizeDelta = dataObjectInspectorWindow.sizeDelta;
            Vector2 sizeDelta = eventData.position - this.lastFrameDragPos_Screen;
            sizeDelta.y *= -1; //Inverse
            Vector2 newSizeDelta = currentSizeDelta + sizeDelta;
            newSizeDelta = ClampToScreen(newSizeDelta);
            dataObjectInspectorWindow.sizeDelta = newSizeDelta;
            lastFrameDragPos_Screen = eventData.position;
        }

        private Vector2 ClampToScreen(Vector2 newSizeDelta)
        {
            float width = Screen.width;
            float height = Screen.height;

            newSizeDelta.x = Mathf.Clamp(newSizeDelta.x, this.minSize, Screen.width - this.dataObjectInspectorWindow.position.x);
            newSizeDelta.y = Mathf.Clamp(newSizeDelta.y, this.minSize, this.dataObjectInspectorWindow.position.y);

            return newSizeDelta;
        }
    }
}