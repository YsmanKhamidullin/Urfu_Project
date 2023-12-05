using System.Collections.Generic;
using System.Linq;
using Algorithms;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Generators
{
    public class RandomWalkRoomGenerator : MonoBehaviour
    {
        [SerializeField] private RoomSettings settings;
        [SerializeField] private Transform roomParentTransform;
        private HashSet<Vector2Int> _path;

        [Button]
        public void GenerateRoom()
        {
            Clear();

            Vector2Int startPos = settings.StartPos;
            int iterationWalkCount = settings.IterationWalkCount;
            _path = AlgorithmRandomWalk.GeneratePath(startPos, iterationWalkCount);

            var iterationStartPos = startPos;
            int iterations = settings.Iterations;
            Cube cellPrefab = settings.CellPrefab;

            for (int i = 0; i < iterations; i++)
            {
                bool iterationFromRandomPos = settings.IterationFromRandomPos;
                if (iterationFromRandomPos)
                {
                    iterationStartPos = _path.ElementAt(Random.Range(0, _path.Count));
                }

                _path.UnionWith(AlgorithmRandomWalk.GeneratePath(iterationStartPos, iterationWalkCount));
            }

            foreach (var pathPos in _path)
            {
                var cellPos = new Vector3(pathPos.x, 0, pathPos.y);
                var cell = Instantiate(cellPrefab, cellPos, cellPrefab.transform.rotation, roomParentTransform);
            }
        }

        [Button]
        private void Clear()
        {
            var childCount = roomParentTransform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                DestroyImmediate(roomParentTransform.GetChild(0).gameObject);
            }
        }
    }
}