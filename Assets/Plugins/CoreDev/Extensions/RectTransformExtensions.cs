using UnityEngine;

namespace CoreDev.Extensions
{
    public static class RectTransformExtensions
    {
        public static void SetAnchorY01(this RectTransform rectTransform, float y01)
        {
            y01 = Mathf.Clamp01(y01);

            float maxX = rectTransform.anchorMax.x;
            float minX = rectTransform.anchorMin.x;
            float maxY = y01;
            float minY = y01;

            rectTransform.anchorMax = new Vector2(maxX, maxY);
            rectTransform.anchorMin = new Vector2(minX, minY);
        }

        public static void SetAnchorX01(this RectTransform rectTransform, float x01)
        {
            x01 = Mathf.Clamp01(x01);

            float maxX = x01;
            float minX = x01;
            float maxY = rectTransform.anchorMax.y;
            float minY = rectTransform.anchorMin.y;

            rectTransform.anchorMax = new Vector2(maxX, maxY);
            rectTransform.anchorMin = new Vector2(minX, minY);
        }

        public static void SetParentAndZeroOutValues(this RectTransform rectTransform, Transform parent, bool maximizeToFitParent)
        {
            rectTransform.SetParent(parent);
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localEulerAngles = Vector3.zero;
            rectTransform.localScale = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;

            if (maximizeToFitParent)
            {
                rectTransform.MaximizeToFitParent();
            }
        }

        public static void MaximizeToFitParent(this RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector3.zero;
            rectTransform.anchorMax = Vector3.one;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
        }
    }
}