using System.Collections.Generic;
using UnityEngine;

namespace Algorithms
{
    public static class AlgorithmRandomWalk
    {
        public static HashSet<Vector2Int> GeneratePath(Vector2Int startPos, int pathLenght)
        {
            var path = new HashSet<Vector2Int> { startPos };

            var prevPos = startPos;
            for (var i = 0; i < pathLenght; i++)
            {
                var newPos = prevPos + Direction2D.GetRandom();
                path.Add(newPos);
                prevPos = newPos;
            }

            return path;
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