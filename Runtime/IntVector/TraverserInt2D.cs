using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyTools.IntVector
{
    /// <summary>
    /// A class for traversing a grid or graph structure.
    /// </summary>
    [Serializable]
    public class TraverserInt2D : ICloneable
    {
        private readonly Vector2Int[] CardinalDirections = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

        public Vector2Int position;
        public LoopInt direction;
        public Vector2Int directionVector => CardinalDirections[direction.value];
        private List<(Vector2Int, int)> history = new();
        public int stepLength;

        public int stepCount { get; private set; }

        public TraverserInt2D(Vector2Int startPosition, int startDirection, int stepLength = 1)
        {
            position = startPosition;
            direction = new(startDirection, 0, 3);
            this.stepLength = stepLength;
            stepCount = 0;
        }

        public Vector2Int Step()
        {
            history.Add((position, direction.value));
            position += CardinalDirections[direction.value] * stepLength;
            stepCount++;
            return position;
        }

        public Vector2Int PeekStep()
        {
            return position + CardinalDirections[direction.value] * stepLength;
        }

        public Vector2Int PeekStep2()
        {
            return position + CardinalDirections[direction.value] * stepLength * 2;
        }

        public bool Undo()
        {
            if (history.Count == 0)
                return false;
            var lastState = history[history.Count - 1];
            history.RemoveAt(history.Count - 1);
            position = lastState.Item1;
            direction.SetValue(lastState.Item2);
            stepCount--;
            return true;
        }

        public void TurnLeft()
        {
            direction--;
        }

        public void TurnRight()
        {
            direction++;
        }

        public void TurnTo(int newDirection)
        {
            direction.SetValue(newDirection);
        }

        public int ClearStepCount()
        {
            int count = stepCount;
            stepCount = 0;
            return count;
        }

        public object Clone()
        {
            TraverserInt2D clone = new TraverserInt2D(position, direction.value, stepLength);
            clone.stepCount = stepCount;
            clone.history = new List<(Vector2Int, int)>(history);
            return clone;
        }
    }
}