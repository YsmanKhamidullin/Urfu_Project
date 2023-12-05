using UnityEngine;

[CreateAssetMenu(menuName = "Game/RoomSettings")]
public class RoomSettings : ScriptableObject
{
    public Vector2Int StartPos => startPos;

    public int Iterations => iterations;

    public int IterationWalkCount => iterationWalkCount;

    public bool IterationFromRandomPos => iterationFromRandomPos;

    public Cube CellPrefab => cellPrefab;

    [SerializeField] private Vector2Int startPos = Vector2Int.zero;
    [SerializeField] private int iterations;
    [SerializeField] private int iterationWalkCount = 15;
    [SerializeField] private bool iterationFromRandomPos = true;
    [SerializeField] private Cube cellPrefab;
}