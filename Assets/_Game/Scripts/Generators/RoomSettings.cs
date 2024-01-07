using UnityEngine;

[CreateAssetMenu(menuName = "Game/RoomSettings")]
public class RoomSettings : ScriptableObject
{
    public Vector2Int StartPos => startPos;

    public int Iterations => iterations;

    public int IterationWalkCount => iterationWalkCount;
    public float MaxDistance => maxDistance;

    public bool IterationFromRandomPos => iterationFromRandomPos;

    public FloorCell CellPrefab => cellPrefab;

    [SerializeField] private Vector2Int startPos = Vector2Int.zero;
    [SerializeField] private int iterations;
    [SerializeField] private int iterationWalkCount = 15;
    [SerializeField, Tooltip("If iteration reach maxDistance then random walk will teleport to startPos")] private float maxDistance = 0;
    [SerializeField] private bool iterationFromRandomPos = true;
    [SerializeField] private FloorCell cellPrefab;
}