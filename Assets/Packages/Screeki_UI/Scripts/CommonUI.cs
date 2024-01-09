using Helpers.Promises;
using UnityEngine;

namespace Windows
{
    public class CommonUI : MonoBehaviour
    {
        public IPromise<LobbyWindow> ShowLobbyWindow()
        {
            return Root.UIManager.StackingShow<LobbyWindow>();
        }

        public void ShowGameplayWindow()
        {
            Root.UIManager.StackingShow<GameplayWindow>();
        }


        // public void ShowCameraWindow()
        // {
        //     Root.UIManager.StackingShow<CameraWindow>();
        // }
        //
        // public void ShowClubProfileWindow(string description, string logoUrl, string title, string shortId)
        // {
        //     var initData = new ClubProfileWindow.InitData()
        //     {
        //         Description = description,
        //         LogoUrl = logoUrl,
        //         Title = title,
        //         ShortId = shortId,
        //     };
        //     Root.UIManager.StackingShow<ClubProfileWindow, ClubProfileWindow.InitData>(initData);
        // }
    }
}