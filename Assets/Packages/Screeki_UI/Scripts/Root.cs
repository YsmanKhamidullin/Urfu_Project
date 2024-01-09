using Windows;
using UIManagement;
using UnityEngine;

public class Root : MonoBehaviour
{
    public static UIManager UIManager => _instance.uiManager;
    public static GameController GameManager => _instance.gameManager;
    public static CommonUI CommonUI => _instance.commonUI;


    [SerializeField] private UIManager uiManager = null;
    [SerializeField] private GameController gameManager = null;
    [SerializeField] private CommonUI commonUI = null;

    private static Root _instance;

    private void Awake()
    {
        _instance = this;
    }

    private void GoGameStart()
    {
        CommonUI.ShowGameplayWindow();
        gameManager.StartGame();
    }

    private void Start()
    {
        //TODO May be error with multi subscription
        CommonUI.ShowLobbyWindow().Done((w) => { w.OnGameStartButtonPressed += GoGameStart; });
    }
}