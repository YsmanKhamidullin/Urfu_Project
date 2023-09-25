using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public RoomSettings Settings => settings;
    public Transform RoomParentTransform => roomParentTransform;

    [SerializeField] private RoomSettings settings;
    [SerializeField] private RoomGeneratorGizmos gizmosPreview;
    [SerializeField] private Transform roomParentTransform;

    [Button]
    public void CreatePreview()
    {
        int childCount = roomParentTransform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            DestroyImmediate(roomParentTransform.GetChild(0).gameObject);
        }

        GameObject prefab = settings.cellPrefab;
        List<Vector3> poses = gizmosPreview.PreviewPositions;
        for (int i = 0; i < poses.Count; i++)
        {
            Instantiate(prefab, poses[i], prefab.transform.rotation, roomParentTransform);
        }
    }
}