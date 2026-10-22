using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTools
{
    /// <summary>
    /// A utility class for handling array-based maps in Unity. Supports dynamic resizing and negative indices.
    /// </summary>
    [Serializable]
    public class ArrayMap<T> : IEnumerable<T>
    {
        private List<List<T>> data = new();
        private int xOffset = 0;
        private int yOffset = 0;
        private int maxWidth;
        private int maxHeight;

        public bool getExtend;

        public int Width => data.Count;
        public int Height => data.Count > 0 ? data[0].Count : 0;

        public int minX => -xOffset;
        public int maxX => Width - xOffset - 1;
        public int minY => -yOffset;
        public int maxY => Height - yOffset - 1;

        public ArrayMap(bool getExtend, int maxWidth, int maxHeight)
        {
            this.getExtend = getExtend;
            this.maxWidth = maxWidth;
            this.maxHeight = maxHeight;
        }

        public T this[Vector2Int vector2]
        {
            get => Get(vector2);
            set => Set(vector2, value);
        }

        public T this[int x, int y]
        {
            get => Get(x, y);
            set => Set(x, y, value);
        }

        public T Get(int x, int y)
        {
            if (getExtend)
            {
                //UnityEngine.Debug.Log($"Get-extending {x}, {y}");
                EnsureCapacity(x, y);
            }
            if (x < minX || x > maxX || y < minY || y > maxY)
                throw new IndexOutOfRangeException($"Index out of range: ({x},{y})");
            return data[x + xOffset][y + yOffset];
        }

        public T Get(Vector2Int vector2)
        {
            return Get(vector2.x, vector2.y);
        }

        public bool TryGet(int x, int y, out T value)
        {
            if (x < minX || x > maxX || y < minY || y > maxY)
            {
                value = default;
                return false;
            }
            value = data[x + xOffset][y + yOffset];
            return true;
        }

        public bool TryGet(Vector2Int vector2, out T value)
        {
            return TryGet(vector2.x, vector2.y, out value);
        }

        public void Set(int x, int y, T value)
        {
            EnsureCapacity(x, y);
            //UnityEngine.Debug.Log($"write into ({x}, {y})");
            data[x + xOffset][y + yOffset] = value;
        }

        public void Set(Vector2Int vector2, T value)
        {
            Set(vector2.x, vector2.y, value);
        }

        public void Clear()
        {
            data.Clear();
            xOffset = 0;
            yOffset = 0;
        }

        public List<T> GetNeighbors(int x, int y, bool returnDefaultValues = true)
        {
            List<T> rst = new();
            int[][] directions = new int[][]
            {
                new int[]{-1, 0}, // left
                new int[]{1, 0},  // right
                new int[]{0, -1}, // down
                new int[]{0, 1}   // up
            };
            foreach (var dir in directions)
            {
                if (TryGet(x + dir[0], y + dir[1], out T value))
                    rst.Add(value);
            }
            if (!returnDefaultValues)
                rst.RemoveAll(v => v == null || v.Equals(default(T)));
            return rst;
        }

        public List<T> GetNeighbors(Vector2Int vector2, bool returnDefaultValues = true)
        {
            return GetNeighbors(vector2.x, vector2.y, returnDefaultValues);
        }

        public List<T> GetAround(int x, int y, bool returnDefaultValues = true)
        {
            List<T> rst = new();
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;
                    if (TryGet(x + dx, y + dy, out T value))
                        rst.Add(value);
                }
            if (!returnDefaultValues)
                rst.RemoveAll(v => v == null || v.Equals(default(T)));
            return rst;
        }

        public List<T> GetAround(Vector2Int vector2, bool returnDefaultValues = true)
        {
            return GetAround(vector2.x, vector2.y, returnDefaultValues);
        }

        private void EnsureCapacity(int x, int y)
        {
            int targetWidth = System.Math.Abs(x + xOffset) + 1;
            int targetHeight = data.Count > 0 ? System.Math.Abs(y + yOffset) + 1 : 0;

            if (targetWidth > maxWidth || targetHeight > maxHeight)
                throw new InvalidOperationException
                    ($"ArrayMap extending out of bound: {targetWidth}x{targetHeight}");

            //UnityEngine.Debug.Log($"Extending to ({targetWidth}, {targetHeight})");

            // extend X axis
            while (x + xOffset < 0)
            {
                data.Insert(0, NewColumn());
                xOffset++;
            }
            while (x + xOffset >= data.Count)
            {
                data.Add(NewColumn());
            }

            // extend Y axis
            while (data.Count > 0 && y + yOffset < 0)
            {
                foreach (var col in data)
                    col.Insert(0, default);
                yOffset++;
            }
            while (data.Count > 0 && y + yOffset >= data[0].Count)
            {
                foreach (var col in data)
                    col.Add(default);
            }
        }

        private List<T> NewColumn()
        {
            int h = data.Count > 0 ? data[0].Count : 0;
            var col = new List<T>(h);
            for (int i = 0; i < h; i++)
                col.Add(default);
            return col;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    yield return data[i][j];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string DisplayCellsStatus()
        {
            if (Width == 0 || Height == 0)
                return "[Empty ArrayMap]";

            System.Text.StringBuilder sb = new();

            // label corners
            sb.AppendLine($"ArrayMap<{typeof(T).Name}> [{minX},{minY}] (left upper) ~ [{maxX},{maxY}] (right lower)");

            // content
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    T value;
                    bool exists = TryGet(x, y, out value);
                    // default(T) treated as non-exists
                    if (!exists || value == null || value.Equals(default(T)))
                        sb.Append(' ');
                    else
                        sb.Append('O');
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}