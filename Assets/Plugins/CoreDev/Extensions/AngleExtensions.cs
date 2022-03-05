using System;
using UnityEngine;

namespace CoreDev.Extensions
{
    public static class AngleExtensions
    {
        /// <summary>
        /// Calculates the angle-in-degrees between two Vectors
        /// </summary>
        /// <param name="fromDir">The first Vector from which the angle is taken</param>
        /// <param name="toDir">The second Vector that makes up the angle we are measuring</param>
        /// <param name="upAxis">The upward axis of the angle. Clockwise is positive and vice versa</param>
        /// <param name="range360">Indicates if the return value should be betwen 0 and 360 or -180 and 180</param>
        /// <returns>Angle and degrees between the two provided vectors</returns>
        public static float AngleTo(this Vector3 fromDir, Vector3 toDir, Vector3 upAxis, bool range360)
        {
            Vector3 fromDirOnPlane = Vector3.ProjectOnPlane(fromDir, upAxis);
            Vector3 toDirOnPlane = Vector3.ProjectOnPlane(toDir, upAxis);
            float angleBetween = Vector3.Angle(fromDirOnPlane, toDirOnPlane);
            Vector3 crossProductNormalized = Vector3.Cross(fromDirOnPlane, toDirOnPlane).normalized;

            if (crossProductNormalized != upAxis)
            {
                if (range360)
                {
                    angleBetween = 360 - angleBetween;
                }
                else
                {
                    angleBetween = -1 * angleBetween;
                }
            }

            return angleBetween;
        }


        /// <summary>
        /// Clamps a given angle (degrees) between 0 and 360
        /// </summary>
        /// <param name="angle">Angle in degrees</param>
        /// <returns>Angle (degrees) clamped between 0 and 360</returns>
        public static float Normalize0To360(this float angle)
        {
            float negToPstve360 = angle % 360;
            float normalizedAngle = (negToPstve360 < 0) ? negToPstve360 + 360 : negToPstve360;

            return normalizedAngle;
        }

        /// <summary>
        /// Clamps a given angle (degrees) between 0 and 360
        /// </summary>
        /// <param name="angle">Angle in degrees</param>
        /// <returns>Angle (degrees) clamped between 0 and 360</returns>
        public static double Normalize0To360(this double angle)
        {
            double negToPstve360 = angle % 360;
            double normalizedAngle = (negToPstve360 < 0) ? negToPstve360 + 360 : negToPstve360;

            return normalizedAngle;
        }


        /// <summary>
        /// Clamps a given angle (degrees) between -180 and 180
        /// </summary>
        /// <param name="angle">Angle in degrees</param>
        /// <returns>Angle (degrees) clamped between -180 and 180</returns>
        public static float NormalizeNegPtv180(this float angle)
        {
            float negToPstve360 = angle % 360;
            float normalizedAngle = negToPstve360;

            if (normalizedAngle < -180)
            {
                normalizedAngle += 360;
            }
            if (normalizedAngle > 180)
            {
                normalizedAngle -= 360;
            }

            return normalizedAngle;
        }

        /// <summary>
        /// Clamps a given angle (degrees) between -180 and 180
        /// </summary>
        /// <param name="angle">Angle in degrees</param>
        /// <returns>Angle (degrees) clamped between -180 and 180</returns>
        public static double NormalizeNegPtv180(this double angle)
        {
            double negToPstve360 = angle % 360;
            double normalizedAngle = negToPstve360;

            if (normalizedAngle < -180)
            {
                normalizedAngle += 360;
            }
            if (normalizedAngle > 180)
            {
                normalizedAngle -= 360;
            }

            return normalizedAngle;
        }

        /// <summary>
        /// Clamps a given angle (degrees) between -PI to PI
        /// </summary>
        /// <param name="angle">Angle in radians</param>
        /// <returns>Angle (degrees) clamped between 0 and 360</returns>
        public static double NormalizeNegPIToPI(this double angle)
        {
            while (angle > Math.PI)
            {
                angle = angle - 2 * Math.PI;
            }

            while (angle <= -Math.PI)
            {
                angle = angle + 2 * Math.PI;
            }

            return angle;
        }

        /// <summary>
        //Get bearing from 0 north given a vector
        /// </summary>
        public static double GetBearing(double x, double y)
        {
            //https://stackoverflow.com/questions/2769860/code-coordinates-to-match-compass-bearings
            return Math.Atan2(x, y); //swap the x and y coordinates to get 0 north
        }
    }
}