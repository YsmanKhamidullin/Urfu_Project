using System.Collections.Generic;
using System.Linq;
using Generators;
using NaughtyAttributes;
using UnityEngine;

public class RoomEnvironmentGenerator : MonoBehaviour
{
    [SerializeField] private RandomWalkRoomGenerator randomWalkRoomGenerator;
    [SerializeField] private List<Material> borderColor;
    [SerializeField] private int wallNeighbors = 3;
    private List<FloorCell> _floor => randomWalkRoomGenerator.Floor;


    [Button]
    public void Generate()
    {
        GenerateColours();
        GenerateWalls();
        GenerateDoors();
    }

    private void GenerateColours()
    {
        foreach (var floorCell in _floor)
        {
            floorCell.SetMeshMaterial(borderColor[floorCell.NearCellsCount - 1]);
        }
    }

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
    }
}