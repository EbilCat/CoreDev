using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CoreDev.DataObjectInspector
{
    public class HorizontalResizeHandle : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private LayoutElement leftMostLayoutElement;
        [SerializeField] private float normalSize = 2.5f;
        [SerializeField] private float expandedSize = 5.0f;

        LayoutElement thisLayoutElement;
        private Vector2 startDragPos_Screen;
        private float startingPreferredWidth;


//*====================
//* UNITY
//*====================
        private void Awake()
        {
            this.thisLayoutElement = this.GetComponent<LayoutElement>();
        }


//*====================
//* UI Handlers
//*====================
        public void OnPointerDown(PointerEventData eventData)
        {
            this.startDragPos_Screen = eventData.position;
            this.startingPreferredWidth = this.leftMostLayoutElement.preferredWidth;
        }

        public void OnDrag(PointerEventData eventData)
        {
            this.leftMostLayoutElement.preferredWidth = startingPreferredWidth + (eventData.position.x - startDragPos_Screen.x);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.leftMostLayoutElement.preferredWidth -= expandedSize / 4.0f;
            thisLayoutElement.minWidth = expandedSize;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.leftMostLayoutElement.preferredWidth += expandedSize / 4.0f;
            thisLayoutElement.minWidth = normalSize;
        }
    }
}