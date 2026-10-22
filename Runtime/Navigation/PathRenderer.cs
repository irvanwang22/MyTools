using UnityEngine;
using UnityEngine.AI;

namespace MyTools.Navigation
{
    public class PathRenderer : MonoBehaviour
    {
        public LineRenderer reachableLineRenderer;
        public LineRenderer unreachableLineRenderer;

        public void ClearPath()
        {
            reachableLineRenderer.positionCount = 0;
            unreachableLineRenderer.positionCount = 0;
        }

        public void RenderPath(NavMeshPath path)
        {
            if (path == null) return;
            reachableLineRenderer.positionCount = path.corners.Length;
            reachableLineRenderer.SetPositions(path.corners);
            unreachableLineRenderer.positionCount = 0;
        }

        public void RenderPath(NavMeshPath path, float maxMoveDistance)
        {
            if (path == null) return;
            LimitedPath limitedPath = new LimitedPath(path, maxMoveDistance);
            RenderPath(limitedPath);
        }

        public void RenderPath(LimitedPath path)
        {
            if (path.originalPath == null) return;
            reachableLineRenderer.positionCount = path.reachableCorners.Length;
            reachableLineRenderer.SetPositions(path.reachableCorners);
            unreachableLineRenderer.positionCount = path.unreachableCorners.Length;
            unreachableLineRenderer.SetPositions(path.unreachableCorners);
        }
    }
}