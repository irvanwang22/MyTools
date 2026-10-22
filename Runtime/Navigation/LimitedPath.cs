using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

namespace MyTools.Navigation
{
    public struct LimitedPath
    {
        public NavMeshPath originalPath { get; private set; }
        public float maxMoveDistance { get; private set; }

        public Vector3[] reachableCorners { get; private set; }
        public Vector3[] unreachableCorners { get; private set; }
        
        public LimitedPath(NavMeshPath originalPath, float maxMoveDistance)
        {
            this.originalPath = originalPath;
            this.maxMoveDistance = maxMoveDistance;

            List<Vector3> reachableCorners = new();
            List<Vector3> unreachableCorners = new();
            reachableCorners.Add(originalPath.corners[0]);

            float totalDistance = 0f;
            bool foundUnreachable = false;

            for (int i = 1; i < originalPath.corners.Length; i++)
            {
                float curDistance = Vector3.Distance(originalPath.corners[i - 1], originalPath.corners[i]);
                if (totalDistance + curDistance <= (float)maxMoveDistance)
                {
                    reachableCorners.Add(originalPath.corners[i]);
                }
                else
                {
                    if (!foundUnreachable)  // first unreachable corner
                    {
                        foundUnreachable = true;
                        float remainingMoveDistance = (float)maxMoveDistance - totalDistance;
                        Vector3 direction = (originalPath.corners[i] - originalPath.corners[i - 1]).normalized;
                        Vector3 splitPoint = originalPath.corners[i - 1] + direction * remainingMoveDistance;
                        reachableCorners.Add(splitPoint);
                        unreachableCorners.Add(splitPoint);
                        unreachableCorners.Add(originalPath.corners[i]);
                    }
                    else  // remaining unreachable corners
                    {
                        unreachableCorners.Add(originalPath.corners[i]);
                    }
                }
                totalDistance += curDistance;
            }

            this.reachableCorners = reachableCorners.ToArray();
            this.unreachableCorners = unreachableCorners.ToArray();
        }
    }
}