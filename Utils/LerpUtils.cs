using System;
using CoreDev.Extensions;
using UnityEngine;

namespace CoreDev.Utils
{
    public class LerpUtils
    {
        public static void Lerp(Vector3 startValue, Vector3 endValue, ref float currentSecs, float lerpTotalSecs, Action<Vector3> setValueCallback, bool affectedByTimeScale = false, Action onLerpCompletedCallback = null)
        {
            currentSecs += (affectedByTimeScale) ? Time.deltaTime : Time.unscaledDeltaTime;
            currentSecs = Mathf.Clamp(currentSecs, 0.0f, lerpTotalSecs);

            Vector3 from = startValue;
            Vector3 to = endValue;
            Vector3 lerpVal = Vector3.Lerp(from, to, currentSecs / lerpTotalSecs);

            setValueCallback(lerpVal);
            if (onLerpCompletedCallback != null && currentSecs.AlmostEquals(lerpTotalSecs)) { onLerpCompletedCallback(); }
        }

        public static void Lerp(float startValue, float endValue, ref float currentSecs, float lerpTotalSecs, Action<float> setValueCallback, bool affectedByTimeScale = false, Action onLerpCompletedCallback = null)
        {
            currentSecs += (affectedByTimeScale) ? Time.deltaTime : Time.unscaledDeltaTime;
            currentSecs = Mathf.Clamp(currentSecs, 0.0f, lerpTotalSecs);

            float from = startValue;
            float to = endValue;
            float lerpVal = Mathf.Lerp(from, to, currentSecs / lerpTotalSecs);

            setValueCallback(lerpVal);
            if (onLerpCompletedCallback != null && currentSecs.AlmostEquals(lerpTotalSecs)) { onLerpCompletedCallback(); }
        }

        public static void Lerp(double startValue, double endValue, ref float currentSecs, float lerpTotalSecs, Action<double> setValueCallback, bool affectedByTimeScale = false, Action onLerpCompletedCallback = null)
        {
            currentSecs += (affectedByTimeScale) ? Time.deltaTime : Time.unscaledDeltaTime;
            currentSecs = Mathf.Clamp(currentSecs, 0.0f, lerpTotalSecs);

            double from = startValue;
            double to = endValue;
            double lerpVal = Lerp(from, to, currentSecs / lerpTotalSecs);

            setValueCallback(lerpVal);
            if (onLerpCompletedCallback != null && currentSecs.AlmostEquals(lerpTotalSecs))
            {
                onLerpCompletedCallback();
            }
        }

        public static double Lerp(double a, double b, float time)
        {
            return a + (b - a) * Clamp01(time);
        }

        private static double Clamp01(double value)
        {
            if (value < 0.0) { return 0.0d; }
            if (value > 1.0) { return 1.0d; }
            else { return value; }
        }
    }
}