using System;
using Helpers.UI.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Windows
{
    [VisibilityThroughWindow(VisibilityThroughWindowMode.NothingIsVisible)]
    public class LobbyWindow : BaseWindow<object>
    {
        public event Action OnGameStartButtonPressed = () => { };
        [SerializeField] private Button gameStartButton = null;

        public override void OnLoad()
        {
            base.OnLoad();
            gameStartButton.onClick.AddListener(() => OnGameStartButtonPressed());
        }
    }
}