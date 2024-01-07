using NaughtyAttributes;
using UnityEngine;

public class FloorCell : MonoBehaviour
{
    public int NearCellsCount => nearCellsCount;
    public CellType CellType => cellType;

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private CellTypeGenerator cellTypeGenerator;

    [SerializeField, ReadOnly] private int nearCellsCount;
    [SerializeField, ReadOnly] private CellType cellType = CellType.Floor;


    public void SetNearCellsCount(int count)
    {
        nearCellsCount = count;
    }

    public void SetMeshMaterial(Material material)
    {
        meshRenderer.sharedMaterial = material;
    }

    public void MakeWall()
    {
        cellTypeGenerator.GenerateWall();
        cellType = CellType.Wall;
    }

    public void MakeNextLevelDoor()
    {
        cellTypeGenerator.GenerateDoor();
        cellType = CellType.Door;
    }
}