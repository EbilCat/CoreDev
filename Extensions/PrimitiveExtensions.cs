using System;
using UnityEngine;

namespace CoreDev.Extensions
{
    public static class PrimitiveExtensions
    {
        public static bool IsEquals(this double a, double b, int significantFigures = 6)
        {
            float limit = 1 / Mathf.Pow(10, significantFigures);
            bool areEqual = Math.Abs(a - b) < limit;
            return areEqual;
        }

        public static bool AlmostEquals(this float a, float b, int significantFigures = 6)
        {
            float limit = 1 / Mathf.Pow(10, significantFigures);
            bool areEqual = Math.Abs(a - b) < limit;
            return areEqual;
        }
        public static bool AlmostEquals(this double a, double b, int significantFigures = 6)
        {
            double limit = 1 / Mathf.Pow(10, significantFigures);
            bool areEqual = Math.Abs(a - b) < limit;
            return areEqual;
        }

        /// <summary>
        /// Converts a value to percentage
        /// </summary>
        /// <param name="value">Value to convert to percentage</param>
        /// <param name="minValue">Lower bound of range</param>
        /// <param name="maxValue">Upper bound of range</param>
        /// <returns>Value in percentage (0 to 1)</returns>
        public static float Get01(this float value, float minValue, float maxValue)
        {
            float clampedValue = Mathf.Clamp(value, minValue, maxValue);
            float valueRange = (maxValue - minValue);
            float value01 = (clampedValue - minValue) / valueRange;
            return value01;
        }

        /// <summary>
        /// Converts a percentage value to an actual value within a range
        /// </summary>
        /// <param name="value01">Percentage value from 0 to 1</param>
        /// <param name="minValue">Lower bound of range</param>
        /// <param name="maxValue">Upper bound of range</param>
        /// <returns>Value of percentage converted from range</returns>
        public static float GetValueOf01(this float value01, float minValue, float maxValue)
        {
            return ((maxValue - minValue) * value01) + minValue;
        }

        /// <summary>
        /// Clamps a value within the positive and negative max value
        /// </summary>
        /// <param name="value">Value to to clamp</param>
        /// <param name="maxValue">Upper bound to clamp</param>
        public static double ClampMaxNegPtv(this double value, double maxValue)
        {
            return value.Clamp(-maxValue, maxValue);
        }

        /// <summary>
        /// Converts a percentage value to an actual value within a range
        /// </summary>
        /// <param name="value">Value to to clamp</param>
        /// <param name="minValue">Lower bound to clamp</param>
        /// <param name="maxValue">Upper bound to clamp</param>
        public static double Clamp(this double value, double minValue, double maxValue)
        {
            if (value > maxValue) { return maxValue; } 
            else if (value < minValue) { return minValue; }
            return value;
        }
    }
}