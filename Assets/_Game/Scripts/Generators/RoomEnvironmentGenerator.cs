using System;
using System.Collections.Generic;
using System.Linq;
using Generators;
using NaughtyAttributes;
using UnityEngine;

public class RoomEnvironmentGenerator : MonoBehaviour
{
    [SerializeField] private RandomWalkRoomGenerator randomWalkRoomGenerator;
    [SerializeField] private int wallNeighbors = 3;
    private List<FloorCell> _floor => randomWalkRoomGenerator.Floor;


    [Button]
    public void Generate()
    {
        // GenerateWalls();
        GenerateDoors();
        GenerateColours();
    }

    //TODO: Variate floor materials
    private void GenerateColours()
    {
        // foreach (var floorCell in _floor)
        // {
        //     floorCell.SetMeshMaterial(borderColor[floorCell.NearCellsCount - 1]);
        // }
    }

    [Button]
    private void GenerateWalls()
    {
        foreach (var floorCell in _floor)
        {
            var count = floorCell.NearCellsCount;
            if (count <= wallNeighbors)
            {
                floorCell.MakeWall();
            }
        }
    }

    [Button]
    public void GenerateDoors()
    {
        var maxNeighborsCount = _floor.Max(f => f.NearCellsCount);
        var filteredCells = _floor.Where(f => f.CellType == CellType.Floor && f.NearCellsCount == maxNeighborsCount)
            .ToArray();
        var firstCellPos = _floor.First().transform.position;
        var maxDistanceCell = filteredCells.Max(c => Vector3.Distance(firstCellPos, c.transform.position));
        var doorCell = filteredCells.First(f =>
            Math.Abs(Vector3.Distance(firstCellPos, f.transform.position) - maxDistanceCell) < 0.01f);
        doorCell.MakeNextLevelDoor();
    }
}