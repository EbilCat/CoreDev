using System;
using UnityEngine;
using UnityEngine.AI;

namespace CoreDev.Extensions
{
    /// <summary>
    /// As suggested in the naming, this extensions is used for nav mesh related
    /// </summary>
    public class NavMeshExtensions
    {
        /// <summary>
        /// If Range expansion is != 0 then it will recusively expand the range till a valid point is found
        /// </summary>
        /// <param name="center"></param>
        /// <param name="range"></param>
        /// <param name="result"></param>
        /// <param name="rangeExpansion"></param>
        /// <returns></returns>
        public static bool SampleAlternatePosition(Vector3 center, float range, out Vector3 result,float rangeExpansion=0f)
        {
            NavMeshHit hit;
            if(NavMesh.SamplePosition(center, out hit, range, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
            else
            {
                if(rangeExpansion>0f)
                    return(SampleAlternatePosition(center, range+rangeExpansion, out result, rangeExpansion));
                else
                {
                    result = Vector3.zero;
                    return false;
                }
            }
        }
    }
}
