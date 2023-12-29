using System.Collections.Generic;
using UnityEngine;

namespace Algorithms
{
    public static class AlgorithmRandomWalk
    {
        public static HashSet<Vector2Int> GeneratePath(Vector2Int startPos, int pathLength, float maxDistance)
        {
            var path = new HashSet<Vector2Int> { startPos };

            var prevPos = startPos;
            for (var i = 0; i < pathLength; i++)
            {
                var randomDir = Direction2D.GetRandom();
                var newPos = prevPos + randomDir;
                if (TryCheckDistance(startPos, newPos, maxDistance))
                {
                    path.Add(newPos);
                }
                else
                {
                    newPos = prevPos - randomDir;
                    path.Add(newPos);
                }

                prevPos = newPos;
            }

            return path;
        }

        private static bool TryCheckDistance(Vector2Int startPos, Vector2Int newPos, float maxDistance)
        {
            if (maxDistance == 0)
            {
                return true;
            }

            var distance = Vector2Int.Distance(newPos, startPos);
            return distance <= maxDistance;
        }
    }

    public static class Direction2D
    {
        public static List<Vector2Int> Directions = new List<Vector2Int>()
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
        };

        public static Vector2Int GetRandom()
        {
            return Directions[Random.Range(0, Directions.Count)];
        }
    }
}