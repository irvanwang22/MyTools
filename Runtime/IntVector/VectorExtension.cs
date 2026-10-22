using UnityEngine;

namespace MyTools.IntVector
{
    /// <summary>
    /// Extension methods for Vector types.
    /// </summary>
    public static class VectorExtension
    {
        public static Vector2Int RoundToVector2Int(this Vector2 vector)
        {
            return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
        }

        public static Vector3Int RoundToVector3Int(this Vector3 vector)
        {
            return new Vector3Int { x = Mathf.RoundToInt(vector.x), y = Mathf.RoundToInt(vector.y), z = Mathf.RoundToInt(vector.z) };
        }

        public static Vector2Int ToVector2Int(this Vector2 vector)
        {
            return new Vector2Int((int)vector.x, (int)vector.y);
        }

        public static Vector3Int ToIntVector3(this Vector3 vector)
        {
            return new Vector3Int { x = (int)vector.x, y = (int)vector.y, z = (int)vector.z };
        }
    }
}