using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CoreDev.Extensions
{
    public static class UIExtensions
    {
        public static bool Add(this InputField inputField, int valueToAdd, bool isClampInputToPositiveValues = false)
        {
            int currentValue;
            bool canAdd = int.TryParse(inputField.text, out currentValue);
            if (canAdd)
            {
                int newValue = currentValue + valueToAdd;
                if (isClampInputToPositiveValues) { newValue = Math.Max(0, newValue); }
                inputField.text = newValue.ToString();
            }
            else { Debug.LogWarning(string.Format("{0}: {1} failed to add {2}. {1} text is not a numeric string", typeof(UIExtensions), inputField.name, valueToAdd)); }
            return canAdd;
        }

        public static bool Add(this InputField inputField, float valueToAdd, bool isClampInputToPositiveValues = false)
        {
            float currentValue;
            bool canAdd = float.TryParse(inputField.text, out currentValue);
            if (canAdd)
            {
                float newValue = currentValue + valueToAdd;
                if (isClampInputToPositiveValues) { newValue = Math.Max(0f, newValue); }
                inputField.text = newValue.ToString();
            }
            else { Debug.LogWarning(string.Format("{0}: {1} failed to add {2}. {1} text is not a numeric string", typeof(UIExtensions), inputField.name, valueToAdd)); }
            return canAdd;
        }

        public static bool Add(this InputField inputField, double valueToAdd, bool isClampInputToPositiveValues = false)
        {
            double currentValue;
            bool canAdd = double.TryParse(inputField.text, out currentValue);
            if (canAdd)
            {
                double newValue = currentValue + valueToAdd;
                if (isClampInputToPositiveValues) { newValue = Math.Max(0.0, newValue); }
                inputField.text = newValue.ToString();
            }
            else { Debug.LogWarning(string.Format("{0}: {1} failed to add {2}. {1} text is not a numeric string", typeof(UIExtensions), inputField.name, valueToAdd)); }
            return canAdd;
        }

        public static int CompareTextTo(this Text first, Text second, bool isReverse = false, bool isEmptySmaller = false)
        {
            if (first == null && second == null) { return 0; }
            else if (first == null) { return isEmptySmaller ? -1 : 1; }
            else if (second == null) { return isEmptySmaller ? 1 : -1; }

            int compare = first.text.CompareTo(second.text);
            if (isReverse) { compare *= -1; }
            return compare;
        }

        public static int CompareTextAsIntTo(this Text first, Text second, bool isReverse = false, bool isEmptySmaller = false)
        {
            if (first == null && second == null) { return 0; }
            else if (first == null) { return isEmptySmaller ? -1 : 1; }
            else if (second == null) { return isEmptySmaller ? 1 : -1; }
            return CompareAsIntTo(first.text, second.text, isReverse, isEmptySmaller);
        }

        public static int CompareAsIntTo(this string first, string second, bool isReverse = false, bool isEmptySmaller = false)
        {
            if (String.IsNullOrEmpty(first) && String.IsNullOrEmpty(second)) { return 0; }
            else if (String.IsNullOrEmpty(first)) { return isEmptySmaller ? -1 : 1; }
            else if (String.IsNullOrEmpty(second)) { return isEmptySmaller ? 1 : -1; }

            bool valuesAreValid = true;
            int firstInt, secondInt;
            valuesAreValid &= int.TryParse(first, out firstInt);
            valuesAreValid &= int.TryParse(second, out secondInt);

            int compare = 0;
            if (valuesAreValid) { compare = firstInt.CompareTo(secondInt); }
            else { compare = first.CompareTo(second); }

            if (isReverse) { compare *= -1; }
            return compare;
        }
    }
}
