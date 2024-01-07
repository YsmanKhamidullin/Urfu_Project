using UnityEngine;

public class CellTypeGenerator : MonoBehaviour
{
    [Header("Door")] 
    [SerializeField] private Transform doorContainer;
    [SerializeField] private Transform doorPrefab;

    [Header("Wall")] 
    [SerializeField] private int wallHeight = 4;
    [SerializeField] private Transform wallContainer;
    [SerializeField] private Transform wallPrefab;

    public void GenerateWall()
    {
        for (int i = 1; i < wallHeight + 1; i++)
        {
            var posStep = Vector3.up * i;
            var pos = transform.position + posStep;
            var wall = Instantiate(wallPrefab, pos, Quaternion.identity, wallContainer);
        }
    }

    public void GenerateDoor()
    {
        var door = Instantiate(doorPrefab, transform.position, Quaternion.identity, doorContainer);
    }
}