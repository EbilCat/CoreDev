using UnityEngine;

namespace CoreDev.Extensions
{
    public static class CameraExtensions
    {
        public static Vector2 WorldToRectPoint_Local(this Camera cam, Vector3 pos_World, RectTransform rect)
        {
            Vector2 pos_Screen = cam.WorldToScreenPoint(pos_World);

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, pos_Screen, null, out Vector2 pos_Rect))
            {
                return pos_Rect;
            }

            return Vector3Extensions.NaN;
        }
    }
}