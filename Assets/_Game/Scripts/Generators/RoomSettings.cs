using UnityEngine;

[CreateAssetMenu(menuName = "Game/RoomSettings")]
public class RoomSettings : ScriptableObject
{
    public Vector2 gridSize;
    public GameObject cellPrefab;
}