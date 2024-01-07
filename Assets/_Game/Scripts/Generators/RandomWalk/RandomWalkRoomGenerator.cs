using System.Collections.Generic;
using System.Linq;
using Algorithms;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Generators
{
    public class RandomWalkRoomGenerator : MonoBehaviour
    {
        public List<FloorCell> Floor => floor;
        [SerializeField] private RoomSettings settings;
        [SerializeField] private RoomEnvironmentGenerator roomEnvironmentGenerator;
        [SerializeField] private Transform roomParentTransform;
        [SerializeField] private int minBlocksCount = 600;
        [SerializeField] private float nearCellsDistance = 1.7f;
        [SerializeField] private int nearCellsCountRemove = 7;


        [SerializeField, ReadOnly] private List<FloorCell> floor;
        private HashSet<Vector2Int> _path;

        [Button]
        public void GenerateRoom()
        {
            Debug.Log("Room generation");
            Clear();
            GeneratePath();
            // if (_path.Count < minBlocksCount)
            // {
            //     GenerateRoom();
            //     return;
            // }

            FloorCell cellPrefab = settings.CellPrefab;
            foreach (var pathPos in _path)
            {
                var cellPos = new Vector3(pathPos.x, 0, pathPos.y);
                FloorCell cell = Instantiate(cellPrefab, cellPos, cellPrefab.transform.rotation, roomParentTransform);
                var nearCellsCount = _path.Count(c => Vector2Int.Distance(c, pathPos) < nearCellsDistance);
                cell.SetNearCellsCount(nearCellsCount - 1);
                floor.Add(cell);
            }

            roomEnvironmentGenerator.Generate();
        }

        private void GeneratePath()
        {
            Vector2Int startPos = settings.StartPos;
            int iterationWalkCount = settings.IterationWalkCount;
            float maxDistance = settings.MaxDistance;
            _path = AlgorithmRandomWalk.GeneratePath(startPos, iterationWalkCount, maxDistance);

            var iterationStartPos = startPos;
            int iterations = settings.Iterations;

            for (int i = 0; i < iterations; i++)
            {
                bool iterationFromRandomPos = settings.IterationFromRandomPos;
                if (iterationFromRandomPos)
                {
                    iterationStartPos = _path.ElementAt(Random.Range(0, _path.Count));
                }

                _path.UnionWith(AlgorithmRandomWalk.GeneratePath(iterationStartPos, iterationWalkCount, maxDistance));
            }

            _path.RemoveWhere(p =>
            {
                var nearCellsCount = _path.Count(c => Vector2Int.Distance(c, p) < nearCellsDistance);
                return nearCellsCount <= nearCellsCountRemove;
            });
        }

        [Button]
        private void Clear()
        {
            var childCount = roomParentTransform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                DestroyImmediate(roomParentTransform.GetChild(0).gameObject);
            }

            floor.Clear();
        }
    }
}