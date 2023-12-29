using NaughtyAttributes;
using UnityEngine;

public class FloorCell : MonoBehaviour
{
    public int NearCellsCount => nearCellsCount;

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField, ReadOnly] private int nearCellsCount;

    [SerializeField] private int wallLength = 4;
    [SerializeField] private Transform wallContainer;
    [SerializeField] private Transform wallPrefab;

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
        for (int i = 1; i < wallLength + 1; i++)
        {
            var wall = Instantiate(wallPrefab, transform.position + Vector3.up * i, Quaternion.identity,
                wallContainer);
        }
    }
}