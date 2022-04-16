﻿using System.Collections.Generic;
using UnityEngine;

namespace CoreDev.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector2 SetValues(this Vector2 vec, float? x, float? y)
        {
            vec.x = (x != null) ? vec.x = (float)x : vec.x;
            vec.y = (y != null) ? vec.y = (float)y : vec.y;

            return vec;
        }

        public static Vector3 SetValues(this Vector3 vec, float? x, float? y, float? z)
        {
            vec.x = (x != null) ? vec.x = (float)x : vec.x;
            vec.y = (y != null) ? vec.y = (float)y : vec.y;
            vec.z = (z != null) ? vec.z = (float)z : vec.z;

            return vec;
        }

        public static bool AlmostEquals(this Vector3 vec, Vector3 other)
        {
            bool result = true;
            if (!(vec.IsInfinity() && other.IsInfinity()))
            {
                result &= vec.x.AlmostEquals(other.x);
                result &= vec.y.AlmostEquals(other.y);
                result &= vec.z.AlmostEquals(other.z);
            }
            return result;
        }

        public static bool IsInfinity(this Vector3 vec)
        {
            bool result = true;
            result &= float.IsInfinity(vec.x);
            result &= float.IsInfinity(vec.y);
            result &= float.IsInfinity(vec.z);
            return result;
        }

        public static bool GetCentre(this Vector3[] area, out Vector3 centre)
        {
            if (area.Length > 0)
            {
                Bounds bounds = new Bounds(area[0], Vector3.zero);
                for (int i = 1; i < area.Length; i++)
                {
                    bounds.Encapsulate(area[i]);
                }
                centre = bounds.center;
                return true;
            }
            else
            {
                centre = Vector3.zero;
                return false;
            }
        }

        public static Vector3 GetVelocityVectorTo(this Vector3 from, Vector3 to, float velocity = 1f)
        {
            Vector3 vector = (to - from).normalized;
            vector *= velocity;
            return vector;
        }

        public static Vector3 LerpAngle(this Vector3 from, Vector3 to, float progress01)
        {
            Vector3 lerpResult;
            lerpResult.x = Mathf.LerpAngle(from.x, to.x, progress01);
            lerpResult.y = Mathf.LerpAngle(from.y, to.y, progress01);
            lerpResult.z = Mathf.LerpAngle(from.z, to.z, progress01);
            return lerpResult;
        }

        public static Vector3 GetCentre(this IList<Vector3> points)
        {
            Vector3 centerPos = points[0];

            for (int i = 1; i < points.Count; i++)
            {
                centerPos += points[i];
            }

            centerPos /= points.Count;
            return centerPos;
        }

        public static bool HasLineOfSightTo(this Vector3 fromPos_World, Vector3 toPos_World, int obstructionsLayerMask = ~0)
        {
            Vector3 vecToPos = (toPos_World - fromPos_World).normalized;
            float distance = Vector3.Distance(fromPos_World, toPos_World);

            RaycastHit hitInfo;
            if (Physics.Raycast(fromPos_World, vecToPos, out hitInfo, distance, obstructionsLayerMask))
            {
                //Debug.Log(hitInfo.collider.name, hitInfo.collider.gameObject);
                return false;
            }
            return true;
        }

        public static float DistanceTo(this Vector3 fromPos_World, Vector3 toPos_World)
        {
            float distance = Vector3.Distance(fromPos_World, toPos_World);
            return distance;
        }
    }
}
