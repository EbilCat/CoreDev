using System;
using System.Collections.Generic;
using CoreDev.Sequencing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RearrangeableScrollViewItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private Transform scrollViewItemTransform;
    [SerializeField] private Color fillerColor = Color.black;
    [SerializeField] private float dragAlpha = 0.5f;
    private float scrollRectScrollRegion01 = 0.25f;

    private Vector2 pointerDownPos_ScrollRect;
    private Vector2 rectransformOriginalPos_ScrollRect;

    private RectTransform itemRectTransform;
    private Transform scrollViewContentTransform;
    private ScrollRect scrollRect;
    private RectTransform scrollRectTransform;

    private GameObject filler;

    private List<Graphic> activeRaycastTargets = new List<Graphic>();

    private CanvasGroup canvasGroup;
    private float canvasGroupOriginalAlpha;

    private event Action<int> siblingIndexChanged = delegate { };


    //*====================
    //* UNITY
    //*====================
    private void Start()
    {
        if (this.GetComponentInParent<Canvas>().renderMode == RenderMode.ScreenSpaceOverlay)
        {
            mainCam = null;
        }
        else
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }

        if (scrollViewItemTransform == null)
        {
            scrollViewItemTransform = this.transform;
        }

        this.scrollRect = this.GetComponentInParent<ScrollRect>();
        this.scrollRectTransform = scrollRect.transform as RectTransform;

        this.canvasGroup = this.scrollViewItemTransform.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            this.canvasGroup = this.scrollViewItemTransform.gameObject.AddComponent<CanvasGroup>();
        }

        this.scrollViewContentTransform = this.scrollViewItemTransform.parent;
        this.itemRectTransform = this.scrollViewItemTransform as RectTransform;
    }


    //*====================
    //* PUBLIC
    //*====================
    public void RegisterForSiblingIndexChanged(Action<int> callback)
    {
        this.UnregisterFromSiblingIndexChanged(callback);
        siblingIndexChanged += callback;
    }

    public void UnregisterFromSiblingIndexChanged(Action<int> callback)
    {
        siblingIndexChanged -= callback;
    }


    //*====================
    //* EventSystems Interfaces
    //*====================
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Vector2 pointerPos_Screen = eventData.position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.scrollRectTransform, pointerPos_Screen, this.mainCam, out pointerDownPos_ScrollRect);

            Vector3 rectTransformPos_World = this.itemRectTransform.position;
            this.WorldToPosInRect(this.scrollRectTransform, rectTransformPos_World, out rectransformOriginalPos_ScrollRect);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            this.CacheAndDisableActiveRaycastTargets();
            this.CreateFiller();

            this.canvasGroupOriginalAlpha = this.canvasGroup.alpha;
            this.canvasGroup.alpha = dragAlpha;

            this.scrollViewItemTransform.SetParent(this.scrollRectTransform);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Vector2 pointerPos_ScrollRect;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.scrollRectTransform, eventData.position, this.mainCam, out pointerPos_ScrollRect);

            this.UpdateScrollViewItemPos(pointerPos_ScrollRect);
            this.EvaluateSiblingIndex(pointerPos_ScrollRect);
            this.EvaluateScrolling(pointerPos_ScrollRect);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            this.scrollViewItemTransform.SetParent(this.scrollViewContentTransform);
            this.scrollViewItemTransform.localPosition = Vector3.zero;
            this.scrollViewItemTransform.localRotation = Quaternion.identity;
            this.scrollViewItemTransform.localScale = Vector3.one;
            this.ReactivateRaycastTargets();

            int siblingIndex = this.filler.transform.GetSiblingIndex();
            this.scrollViewItemTransform.SetSiblingIndex(siblingIndex);
            this.siblingIndexChanged(siblingIndex);

            this.DestroyFiller();
            this.canvasGroup.alpha = this.canvasGroupOriginalAlpha;
        }
    }


    //*====================
    //* PRIVATE
    //*====================
    private void CreateFiller()
    {
        this.filler = new GameObject("Placeholder");
        LayoutElement layoutElement = filler.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = this.itemRectTransform.rect.height;
        layoutElement.preferredWidth = this.itemRectTransform.rect.width;
        layoutElement.minHeight = this.itemRectTransform.rect.height;
        layoutElement.minWidth = this.itemRectTransform.rect.width;

        filler.transform.SetParent(this.scrollViewContentTransform);
        filler.transform.localPosition = Vector3.zero;
        filler.transform.localRotation = Quaternion.identity;
        filler.transform.localScale = Vector3.one;

        filler.transform.SetSiblingIndex(this.scrollViewItemTransform.GetSiblingIndex());

        Image fillerImage = filler.AddComponent<Image>();
        fillerImage.color = fillerColor;
        fillerImage.raycastTarget = false;
    }

    private void DestroyFiller()
    {
        Destroy(this.filler);
    }

    private void UpdateScrollViewItemPos(Vector2 pointerPos_ScrollRect)
    {
        Vector2 newPos_Canvas = rectransformOriginalPos_ScrollRect - (pointerDownPos_ScrollRect - pointerPos_ScrollRect);
        this.scrollViewItemTransform.localPosition = newPos_Canvas;
    }

    private void EvaluateSiblingIndex(Vector2 pointerPos_ScrollRect)
    {
        RectTransform fillerRectTransform = this.filler.transform as RectTransform;
        int siblingIndex = this.GetSiblingIndexFromPointerPos(pointerPos_ScrollRect, this.filler.transform.GetSiblingIndex());
        this.filler.transform.SetSiblingIndex(siblingIndex);
    }

    private int GetSiblingIndexFromPointerPos(Vector2 pointerPos_ScrollRect, int currentSiblingIndex)
    {
        int siblingIndex = currentSiblingIndex;
        float prevItemYPos_Canvas = GetNeighbouringItemYPos_Canvas(siblingIndex, false);
        float nextItmeYPos_Canvas = GetNeighbouringItemYPos_Canvas(siblingIndex, true);

        if (pointerPos_ScrollRect.y > prevItemYPos_Canvas || pointerPos_ScrollRect.y < nextItmeYPos_Canvas)
        {
            Vector3 fillerPos_World = this.filler.transform.position;
            Vector2 fillerPos_Canvas;
            this.WorldToPosInRect(this.scrollRectTransform, fillerPos_World, out fillerPos_Canvas);

            Vector2 vecToPointer_Canvas = pointerPos_ScrollRect - fillerPos_Canvas;

            if (vecToPointer_Canvas.y > 0)
            {
                siblingIndex = (siblingIndex == 0) ? 0 : siblingIndex - 1;
            }
            else
            {
                siblingIndex = siblingIndex + 1;
            }

            siblingIndex = this.GetSiblingIndexFromPointerPos(pointerPos_ScrollRect, siblingIndex);
        }

        return siblingIndex;
    }

    private void EvaluateScrolling(Vector2 pointerPos_ScrollRect)
    {
        float pointerYPos_ScrollRect = pointerPos_ScrollRect.y + (scrollRectTransform.pivot.y * scrollRectTransform.rect.height); //Nullify effects of pivot position
        float pointerYPos01_ScrollRect = Mathf.Clamp01(pointerYPos_ScrollRect / scrollRectTransform.rect.height); //1 is at the top and 0 is at bottom of viewport

        if (pointerYPos01_ScrollRect < scrollRectScrollRegion01)
        {
            float scrollSpeed01 = (scrollRectScrollRegion01 - pointerYPos01_ScrollRect) / scrollRectScrollRegion01;
            this.scrollRect.verticalNormalizedPosition -= scrollSpeed01 * Time.unscaledDeltaTime;
        }
        else
        if (pointerYPos01_ScrollRect > (1.0f - scrollRectScrollRegion01))
        {
            float scrollSpeed01 = (pointerYPos01_ScrollRect - (1.0f - scrollRectScrollRegion01)) / scrollRectScrollRegion01;
            this.scrollRect.verticalNormalizedPosition += scrollSpeed01 * Time.unscaledDeltaTime;
        }
    }

    private float GetNeighbouringItemYPos_Canvas(int currentIndex, bool downwardTraversal)
    {
        int itemIndex = (downwardTraversal) ? currentIndex + 1 : currentIndex - 1;

        while (itemIndex >= 0 && itemIndex < this.scrollViewContentTransform.childCount)
        {
            Transform nextItemTransform = scrollViewContentTransform.GetChild(itemIndex);

            if (nextItemTransform.gameObject.activeInHierarchy)
            {
                Vector2 pos_Canvas;
                bool result = this.WorldToPosInRect(this.scrollRectTransform, nextItemTransform.position, out pos_Canvas);

                if (result)
                {
                    return pos_Canvas.y;
                }
            }

            itemIndex += (downwardTraversal) ? 1 : -1;
        }

        float infinity = (downwardTraversal) ? float.NegativeInfinity : float.PositiveInfinity;
        return infinity;
    }

    private void CacheAndDisableActiveRaycastTargets()
    {
        this.activeRaycastTargets.Clear();

        Graphic[] raycastTargets = this.scrollViewItemTransform.GetComponentsInChildren<Graphic>();
        foreach (Graphic raycastTarget in raycastTargets)
        {
            if (raycastTarget.raycastTarget == true)
            {
                this.activeRaycastTargets.Add(raycastTarget);
                raycastTarget.raycastTarget = false;
            }
        }
    }

    private void ReactivateRaycastTargets()
    {
        foreach (Graphic raycastTarget in activeRaycastTargets)
        {
            raycastTarget.raycastTarget = true;
        }
        activeRaycastTargets.Clear();
    }

    private bool WorldToPosInRect(RectTransform rectTransform, Vector3 pos_World, out Vector2 pos_Rect)
    {
        Vector2 pos_Screen = (this.mainCam == null) ? pos_World : this.mainCam.WorldToScreenPoint(pos_World);
        bool result = RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, pos_Screen, this.mainCam, out pos_Rect);
        return result;
    }
}
