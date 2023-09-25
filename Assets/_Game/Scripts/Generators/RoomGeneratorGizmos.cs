using System.Collections.Generic;
using UnityEngine;

public class RoomGeneratorGizmos : MonoBehaviour
{
    public List<Vector3> PreviewPositions => previewPositions;
    [SerializeField] private RoomGenerator generator;
    [SerializeField] private List<Vector3> previewPositions;

    private void OnDrawGizmos()
    {
        previewPositions = new List<Vector3>();
        Vector2 size = generator.Settings.gridSize;
        Transform startPos = generator.RoomParentTransform;
        Vector3 cubeScale = new Vector3(1, 0, 1);
        Vector3 centerDistance = new Vector3(size.x/2, 0, size.y/2);
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Vector3 pos = startPos.position + new Vector3(i, 0, j) - centerDistance;
                Gizmos.DrawCube(pos, cubeScale);
                previewPositions.Add(pos);
            }
        }
    }
}