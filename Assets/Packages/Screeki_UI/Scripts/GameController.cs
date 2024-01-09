using Generators;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private RandomWalkRoomGenerator roomGenerator;
    public void StartGame()
    {
        roomGenerator.GenerateRoom();
    }
}
