using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyTools.UI
{
    /// <summary>
    /// A collection of tools for raycasting in Unity UI.
    /// </summary>
    public static class RaycastTools
    {
        public static GameObject GetFirstRaycastObject(Vector3 position, Camera camera)
        {
            PointerEventData data = new(EventSystem.current)
            {
                position = position
            };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);
            if (results.Count > 0)
            {
                return results[0].gameObject;
            }
            return null;
        }
    }
}