using System;
using System.Collections.Generic;
using System.Linq;
using CoreDev.Extensions;

namespace UnityEngine.UI
{
    public class MinimumFlexibleHeightVerticalLayoutGroup : HorizontalOrVerticalLayoutGroup
    {
        private const int axis = 1;   // 0 - Horizontal, 1 - Vertical
        private const bool isVertical = true;

        protected float totalFlexiblePreferred = 0f;
        protected float totalFlexibleMin = 0f;
        protected List<Tuple<RectTransform, Sizes>> flexibleHeightChildrenSizes = new List<Tuple<RectTransform, Sizes>>();
        protected List<RectTransform> sortedFlexibleHeightChildren = new List<RectTransform>();

        public struct Sizes
        {
            public float min;
            public float preferred;
            public float flexible;
        }

        protected MinimumFlexibleHeightVerticalLayoutGroup()
        { }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalcAlongAxis(0, true);
        }

        public override void CalculateLayoutInputVertical()
        {
            CalcAlongVerticalAxis();
        }

        public override void SetLayoutHorizontal()
        {
            SetChildrenAlongAxis(0, true);
        }

        public override void SetLayoutVertical()
        {
            SetChildrenAlongVerticalAxis();
        }

        protected void CalcAlongVerticalAxis()
        {
            float combinedPadding = padding.vertical;
            bool controlSize = m_ChildControlHeight;
            bool useScale = m_ChildScaleHeight;
            bool childForceExpandSize = m_ChildForceExpandHeight;

            float totalNonFlexibleMin = combinedPadding;
            float totalNonFlexiblePreferred = combinedPadding;
            float totalFlexible = 0;

            totalFlexiblePreferred = totalNonFlexiblePreferred;
            totalFlexibleMin = totalNonFlexibleMin;
            flexibleHeightChildrenSizes.Clear();
            sortedFlexibleHeightChildren.Clear();

            float size = rectTransform.rect.size[axis];

            for (int i = 0; i < rectChildren.Count; i++)
            {
                RectTransform child = rectChildren[i];
                float min, preferred, flexible;
                GetChildSizes(child, axis, controlSize, childForceExpandSize, out min, out preferred, out flexible);

                if (useScale)
                {
                    float scaleFactor = child.localScale[axis];
                    min *= scaleFactor;
                    preferred *= scaleFactor;
                    flexible *= scaleFactor;
                }


                if (flexible.AlmostEquals(0f))
                {
                    totalNonFlexiblePreferred += preferred + spacing;
                    totalNonFlexibleMin += min + spacing;
                }
                else
                {
                    totalFlexiblePreferred += preferred + spacing;
                    totalFlexibleMin += min + spacing;

                    flexibleHeightChildrenSizes.Add(Tuple.Create(child,
                    new Sizes
                    {
                        min = min,
                        preferred = preferred,
                        flexible = flexible
                    }));
                }

                // Increment flexible size with element's flexible size.
                totalFlexible += flexible;
            }

            // Remove additional spacing
            if (rectChildren.Count > 0)
            {
                if (rectChildren.Count > flexibleHeightChildrenSizes.Count)
                {
                    totalNonFlexibleMin -= spacing;
                    totalNonFlexiblePreferred -= spacing;
                }
                if (flexibleHeightChildrenSizes.Count > 0)
                {
                    totalFlexiblePreferred -= spacing;
                    totalFlexibleMin -= spacing;
                }
            }

            totalNonFlexiblePreferred = Mathf.Max(totalNonFlexibleMin, totalNonFlexiblePreferred);

            // Recalculate flexible children heights and set as preferred height
            if (controlSize && flexibleHeightChildrenSizes.Count > 0)
            {
                flexibleHeightChildrenSizes.Sort((first, second) => first.Item2.preferred.CompareTo(second.Item2.preferred));
                sortedFlexibleHeightChildren.AddRange(flexibleHeightChildrenSizes.Select(tuple => tuple.Item1));

                float minMaxLerp = 1f;
                float surplusSpace = size - totalNonFlexiblePreferred - totalFlexibleMin;

                if (surplusSpace > 0)   // Calculate additional flexible space and set preferred to Min of preferred vs flexible
                {
                    surplusSpace += totalFlexibleMin;
                    float itemFlexibleMultiplier = 0f;

                    for (int i = 0; i < sortedFlexibleHeightChildren.Count; i++)
                    {
                        if (totalFlexible > 0) { itemFlexibleMultiplier = surplusSpace / totalFlexible; }

                        RectTransform child = sortedFlexibleHeightChildren[i];
                        Sizes childSizes = flexibleHeightChildrenSizes[i].Item2;

                        float childSize = Mathf.Lerp(childSizes.min, childSizes.preferred, minMaxLerp);

                        float flexibleHeight = childSizes.flexible * itemFlexibleMultiplier;
                        childSize = Mathf.Min(childSize, flexibleHeight);

                        surplusSpace -= childSize;
                        totalFlexible -= childSizes.flexible;

                        totalNonFlexiblePreferred = totalNonFlexiblePreferred - childSizes.min + childSize;

                        flexibleHeightChildrenSizes[i] = Tuple.Create(child, new Sizes()
                        {
                            min = childSizes.min,
                            preferred = childSize,
                            flexible = childSizes.flexible
                        });
                    }
                }
                else if (surplusSpace < 0)  // No surplus space, clamp preferred to min
                {
                    for (int i = 0; i < sortedFlexibleHeightChildren.Count; i++)
                    {
                        RectTransform child = sortedFlexibleHeightChildren[i];
                        Sizes childSizes = flexibleHeightChildrenSizes[i].Item2;
                        flexibleHeightChildrenSizes[i] = Tuple.Create(child, new Sizes()
                        {
                            min = childSizes.min,
                            preferred = childSizes.min,
                            flexible = childSizes.flexible
                        });
                    }
                }
            }

            SetLayoutInputForAxis(totalNonFlexibleMin, totalNonFlexiblePreferred, totalFlexible, axis);
        }

        protected void SetChildrenAlongVerticalAxis()
        {
            float size = rectTransform.rect.size[axis];
            bool controlSize = m_ChildControlHeight;
            bool useScale = m_ChildScaleHeight;
            bool childForceExpandSize = m_ChildForceExpandHeight;
            float alignmentOnAxis = GetAlignmentOnAxis(axis);

            float pos = padding.top;
            float itemFlexibleMultiplier = 0;
            float surplusSpace = size - GetTotalPreferredSize(axis);

            float minMaxLerp = 1f;
            float totalMin = GetTotalMinSize(axis);
            float totalPreferred = GetTotalPreferredSize(axis);
            if ((totalMin + totalFlexibleMin) != totalPreferred)
            {
                minMaxLerp = Mathf.Clamp01((size - totalMin - totalFlexibleMin) / (totalPreferred - totalMin));
            }

            for (int i = 0; i < rectChildren.Count; i++)
            {
                RectTransform child = rectChildren[i];
                float min, preferred, flexible;

                if (sortedFlexibleHeightChildren.Contains(child))
                {
                    Sizes childSizes = flexibleHeightChildrenSizes[sortedFlexibleHeightChildren.IndexOf(child)].Item2;
                    min = childSizes.min;
                    preferred = childSizes.preferred;
                    flexible = childSizes.flexible;
                }
                else
                {
                    GetChildSizes(child, axis, controlSize, childForceExpandSize, out min, out preferred, out flexible);
                }

                float scaleFactor = useScale ? child.localScale[axis] : 1f;

                float childSize = Mathf.Lerp(min, preferred, minMaxLerp);
                // childSize += flexible * itemFlexibleMultiplier;
                if (controlSize)
                {
                    // if (flexible.AlmostEquals(0f) == false)
                    // {
                    //     float flexibleHeight = flexible * itemFlexibleMultiplier;
                    //     childSize = Mathf.Min(childSize, flexibleHeight);

                    //     surplusSpace -= childSize;
                    //     itemFlexibleMultiplier = surplusSpace / GetTotalFlexibleSize(axis);
                    //     totalFlexibleSize -= flexible;
                    //     if (totalFlexibleSize > 0)
                    //     { itemFlexibleMultiplier = surplusSpace / totalFlexibleSize; }
                    // }
                    SetChildAlongAxisWithScale(child, axis, pos, childSize, scaleFactor);
                }
                else
                {
                    float totalFlexibleSize = GetTotalFlexibleSize(axis);

                    if (surplusSpace > 0)
                    {
                        if (totalFlexibleSize == 0)
                            pos = GetStartOffset(axis, GetTotalPreferredSize(axis) - padding.vertical);
                        else if (totalFlexibleSize > 0)
                            itemFlexibleMultiplier = surplusSpace / totalFlexibleSize;
                    }

                    childSize += flexible * itemFlexibleMultiplier;
                    float offsetInCell = (childSize - child.sizeDelta[axis]) * alignmentOnAxis;
                    SetChildAlongAxisWithScale(child, axis, pos + offsetInCell, scaleFactor);
                }
                pos += childSize * scaleFactor + spacing;
            }
        }

        /// <summary>
        /// Function copied from HorizontalOrVerticalLayoutGroup
        /// </summary>
        private void GetChildSizes(RectTransform child, int axis, bool controlSize, bool childForceExpand,
            out float min, out float preferred, out float flexible)
        {
            if (!controlSize)
            {
                min = child.sizeDelta[axis];
                preferred = min;
                flexible = 0;
            }
            else
            {
                min = LayoutUtility.GetMinSize(child, axis);
                preferred = LayoutUtility.GetPreferredSize(child, axis);
                flexible = LayoutUtility.GetFlexibleSize(child, axis);
            }

            if (childForceExpand)
                flexible = Mathf.Max(flexible, 1);
        }
    }
}
