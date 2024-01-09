using Helpers.Promises;
using System;
using UnityEngine;

namespace Interfaces
{
    public interface IWindow
    {
        bool Initialized { get; }
        void OnLoad();

        void InitFromUIManager(Func<IPromise> closeWindowRequest);
        IPromise ShowFromUIManager(bool thisWindowIsFirst, Action initAction);
        IPromise HideFromUIManager();
        IPromise SmoothDestroy();

        void OnWindowCoverClick();
        void HandleEscapeButton();

        Transform GetWindowRootTransform();
    }

    public interface IWindow<T> : IWindow
    {
        void Init(T initData);
    }
}
