using System;
using UnityEngine;

namespace CoreDev.Extensions
{
    public static class UnitConversions
    {
        public static float MsToKnots(this float speedMs)
        {
            float speedKnots = speedMs / 0.51444f;
            return speedKnots;
        }

        public static float KnotsToMs(this float speedKnots)
        {
            float speedMs = speedKnots * 0.51444f;
            return speedMs;
        }

        public static float MinsToSecs(this float mins)
        {
            float seconds = mins * 60.0f;
            return seconds;
        }

        public static float SecsToMins(this float secs)
        {
            float mins = secs / 60.0f;
            return mins;
        }

        public static double MinsToSecs(this double mins)
        {
            double seconds = mins * 60.0f;
            return seconds;
        }

        public static double SecsToMins(this double secs)
        {
            double mins = secs / 60.0f;
            return mins;
        }

        public static double RadianToDegrees(this double radian)
        {
            double degrees = (radian / Math.PI) * 180;
            return degrees;
        }

        public static double DegreesToRadian(this double degrees)
        {
            double radian = (degrees / 180.0f) * Math.PI;
            return radian;
        }

        public static float RadianToDegrees(this float radian)
        {
            float degrees = (radian / Mathf.PI) * 180;
            return degrees;
        }

        public static float DegreesToRadian(this float degrees)
        {
            float radian = (degrees / 180.0f) * Mathf.PI;
            return radian;
        }

        public static double MetersToCables(this double meters)
        {
            double cables = System.Math.Ceiling(meters / 185.2);
            return cables;
        }
        public static double CablesToMeters(this double cables)
        {
            double meters = cables * 185.2;
            return meters;
        }
    }
}