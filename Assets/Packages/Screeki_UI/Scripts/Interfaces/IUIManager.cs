using Helpers.Promises;
using System;
using UnityEngine;

namespace Interfaces
{
    public interface IUIManager
    {
        event Action OnStackChanged;

        void Init();
        T WarmUp<T>() where T : class, IWindow;

        IUIManager ClearStack();
        IUIManager AddToStack<T>() where T : class, IWindow<object>;
        IUIManager AddToStack<T, D>(D initData) where T : class, IWindow<D>;

        IPromise<T> StackingShow<T>() where T : class, IWindow<object>;
        IPromise<T> StackingShow<T, D>(D initData) where T : class, IWindow<D>;

        void IsPointHandledByUI(Vector2 screenPosition, out bool handlesClick, out bool handlesDrag);
        bool IsFullscreenWindowShown();
        Type GetActiveWindowType();

        IPromise<bool> ShowErrorMessage(ErrorType type, string message, string buttonActionText, string processMessage,
            string showCancelButton = null);

        void HideErrorMessage(ErrorType type);
    }

    public enum ErrorType
    {
        Disconnect,
        CriticalError,
        Ban
    }
}