using UnityEngine;
using UnityEngine.EventSystems;


namespace CoreDev.DataObjectInspector
{
    public class DataObjectInspectorHeader : MonoBehaviour, IInitializePotentialDragHandler, IDragHandler
    {
        [SerializeField] private RectTransform dataObjectInspectorWindow;
        private Vector2 lastFrameDragPos_Screen;

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            lastFrameDragPos_Screen = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 moveDelta = eventData.position - this.lastFrameDragPos_Screen;
            Vector2 newPos = (Vector2)dataObjectInspectorWindow.position + moveDelta;
            newPos = ClampToScreen(newPos);
            dataObjectInspectorWindow.position = newPos;
            lastFrameDragPos_Screen = eventData.position;
        }

        private Vector2 ClampToScreen(Vector2 newPos)
        {
            float width = Screen.width - this.dataObjectInspectorWindow.rect.width;
            float height = Screen.height;

            newPos.x = Mathf.Clamp(newPos.x, 0, width);
            newPos.y = Mathf.Clamp(newPos.y, this.dataObjectInspectorWindow.rect.height, height);

            return newPos;
        }
    }
}