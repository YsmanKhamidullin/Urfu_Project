using System;
using System.Collections.Generic;
using Helpers;
using Helpers.Promises;
using Helpers.UI;
using Helpers.UI.Attributes;
using Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIManagement
{
    public class UIManager : MonoBehaviour, IUIManager
    {
        public event Action OnStackChanged = () => { };

        [SerializeField] private GraphicRaycaster graphicRayСaster;
        [SerializeField] private RectTransform windowsParent;
        [SerializeField] private WindowCover windowCover;
        [SerializeField] private ErrorMessagePanel criticalErrorPanel;

        // first position in this list (index 0) means top position in windows stack (on top of all)
        private readonly List<WindowStackItem> _windowsStack = new List<WindowStackItem>();
        private IPromise _transitionQueue = Deferred.GetFromPool().Resolve();

        private List<IWindow> _windows = new List<IWindow>();

        private ErrorType? _lastShownErrorType;

        private IPromise StackingHide(Type type)
        {
            _transitionQueue = _transitionQueue.Then(() =>
            {
                for (int i = 0; i < _windowsStack.Count; i++)
                {
                    if (_windowsStack[i].type == type)
                    {
                        _windowsStack.RemoveAt(i);
                        break;
                    }
                }

                return RefreshStackVisibility(false);
            });

            return _transitionQueue;
        }


        #region windows public processes

        private IPromise<T> StackingShow<T>(WindowStackItem stackItem) where T : IWindow
        {
            _transitionQueue = _transitionQueue.Then(() =>
            {
                _windowsStack.Insert(0, stackItem);
                return RefreshStackVisibility(true);
            });

            Deferred<T> result = Deferred<T>.GetFromPool();
            _transitionQueue.Done(() => { result.Resolve(GetExistingWindow<T>()); });
            _transitionQueue.Fail(ex =>
            {
                result.Reject(ex);
                Debug.LogError(ex.Message);
                _transitionQueue = Deferred.GetFromPool().Resolve();
            });
            return result;
        }

        public IPromise<T> StackingShow<T>() where T : class, IWindow<object>
        {
            return StackingShow<T>(new WindowStackItem<T, object>(null));
        }

        public IPromise<T> StackingShow<T, D>(D initData) where T : class, IWindow<D>
        {
            return StackingShow<T>(new WindowStackItem<T, D>(initData));
        }

        public IUIManager ClearStack()
        {
            _windowsStack.Clear();
            return this;
        }

        public Type GetActiveWindowType()
        {
            if (_windowsStack.Count == 0)
                return null;
            return _windowsStack[0].type;
        }

        private IUIManager AddToStack<T>(WindowStackItem stackItem)
        {
            _windowsStack.Insert(0, stackItem);
            return this;
        }

        public IUIManager AddToStack<T>() where T : class, IWindow<object>
        {
            return AddToStack<T>(new WindowStackItem<T, object>(null));
        }

        public IUIManager AddToStack<T, D>(D initData) where T : class, IWindow<D>
        {
            return AddToStack<T>(new WindowStackItem<T, D>(initData));
        }

        private T GetExistingWindow<T>() where T : IWindow
        {
            for (int i = 0, maxi = _windows.Count - 1; i <= maxi; i++)
            {
                if (_windows[i] is T)
                {
                    return (T)(_windows[i]);
                }
            }

            return default(T);
        }

        private IWindow GetExistingWindow(Type type)
        {
            for (int i = 0, maxi = _windows.Count - 1; i <= maxi; i++)
            {
                if (_windows[i].GetType() == type)
                {
                    return _windows[i];
                }
            }

            return null;
        }

        #endregion windows public processes


        #region windows background processes

        private IWindow GetOrCreateWindowInstance(Type type, out Exception exception)
        {
            IWindow existing = GetExistingWindow(type);

            if (existing != null)
            {
                exception = null;
                return existing;
            }
            else
            {
                return InstantiateWindow(type, out exception);
            }
        }

        private IWindow InstantiateWindow(Type type, out Exception exception)
        {
            string pathToWindow = "" + type.Name;

            var prefab = Resources.Load<GameObject>(pathToWindow);
            if (prefab == null)
            {
                string errorText = "cant finwindow resource: '" + pathToWindow + "'";
                Debug.LogError(errorText);
                exception = new Exception(errorText);
                return null;
            }

            IWindow windowInstance = Instantiate(prefab, windowsParent).GetComponent(type) as IWindow;
            _windows.Add(windowInstance);
            windowInstance.InitFromUIManager(() => { return StackingHide(type); });
            windowInstance.OnLoad();
            exception = null;
            return windowInstance;
        }


        private List<IWindow> windowsCacheHideCandidates = new List<IWindow>();
        private List<Transform> windowsCacheVisibleStack = new List<Transform>();
        private List<IPromise> windowsInstantiatePromisesCache = new List<IPromise>();
        private List<IPromise> transitionPromisesCache = new List<IPromise>();
        private HashSet<Type> processedWindowTypesCache = new HashSet<Type>();

        private IPromise RefreshStackVisibility(bool openingNewWindow)
        {
            //// cache all known windows to hide invisible later
            windowsCacheHideCandidates.Clear();
            windowsCacheHideCandidates.AddRange(_windows);

            //// cache all visible windows to sort them later
            windowsCacheVisibleStack.Clear();

            transitionPromisesCache.Clear();
            processedWindowTypesCache.Clear();

            bool isFirstWindow = true;
            bool coverSet = false;
            bool hudExists = false;
            BackgroundType bgTypeRequired = BackgroundType.None;

            //sort priority windows
            while (true)
            {
                bool stackChanged = false;
                for (int i = 0, maxi = _windowsStack.Count - 2; i <= maxi; i++)
                {
                    WindowStackItem top = _windowsStack[i];
                    PriorityWindowAttribute topPriority = AttributeUtil.GetAttribute<PriorityWindowAttribute>(top.type);
                    bool topWindowIsPriority = topPriority == null ? false : topPriority.isPriorityWindow;

                    WindowStackItem bottom = _windowsStack[i + 1];
                    PriorityWindowAttribute bottomPriority =
                        AttributeUtil.GetAttribute<PriorityWindowAttribute>(bottom.type);
                    bool bottomWindowIsPriority = bottomPriority == null ? false : bottomPriority.isPriorityWindow;

                    if (topWindowIsPriority == false && bottomWindowIsPriority == true)
                    {
                        _windowsStack[i] = bottom;
                        _windowsStack[i + 1] = top;
                        stackChanged = true;
                    }
                }

                if (stackChanged == false)
                {
                    break;
                }
            }

            foreach (WindowStackItem windowStackItem in _windowsStack)
            {
                if (processedWindowTypesCache.Contains(windowStackItem.type) == true)
                {
                    continue;
                }
                else
                {
                    processedWindowTypesCache.Add(windowStackItem.type);
                }

                int myStackIndex = windowsCacheVisibleStack.Count;
                windowsCacheVisibleStack.Add(null);

                bool thisWindowIsFirst = isFirstWindow == true;
                Deferred<IWindow> showPromise = Deferred<IWindow>.GetFromPool();
                Deferred<IWindow> windowInstantiatePromise = Deferred<IWindow>.GetFromPool();

                // show window and call init only for first one
                Exception exception;
                IWindow windowInfo = GetOrCreateWindowInstance(windowStackItem.type, out exception);

                if (exception == null)
                {
                    windowsCacheVisibleStack[myStackIndex] = windowInfo.GetWindowRootTransform();
                    windowInstantiatePromise.Resolve(windowInfo);

                    bool withInit = true;

                    // can skip init only for previously inited windows
                    if (windowInfo.Initialized == true)
                    {
                        // don't reinit window if we are going back and window exists
                        if (openingNewWindow == false)
                        {
                            withInit = false;
                        }

                        // don't reinit windows that not currently on top - even if it's visible at background.
                        if (thisWindowIsFirst == false)
                        {
                            withInit = false;
                        }
                    }

                    Action initAction = null;
                    if (withInit == true)
                    {
                        initAction = () => windowStackItem.initAction(windowInfo);
                    }

                    windowInfo
                        .ShowFromUIManager(thisWindowIsFirst, initAction)
                        .Done(() => { showPromise.Resolve(windowInfo); })
                        .Fail(ex => { showPromise.Reject(ex); });
                }
                else
                {
                    showPromise.Reject(exception);
                    windowInstantiatePromise.Reject(exception);
                }

                transitionPromisesCache.Add(showPromise);
                windowsInstantiatePromisesCache.Add(windowInstantiatePromise);

                // remove shown window from hide list
                for (int i = windowsCacheHideCandidates.Count - 1; i >= 0; i--)
                {
                    if (windowsCacheHideCandidates[i].GetType() == windowStackItem.type)
                    {
                        windowsCacheHideCandidates.RemoveAt(i);
                    }
                }

                // ask for required background type
                RequiresBackgroundAttribute attrBG =
                    AttributeUtil.GetAttribute<RequiresBackgroundAttribute>(windowStackItem.type);
                if (attrBG != null && bgTypeRequired == BackgroundType.None)
                {
                    bgTypeRequired = attrBG.backgroundType;
                }

                // if we showing background, we need not check other windows
                if (bgTypeRequired != BackgroundType.None)
                {
                    break;
                }

                bool stopStackPrcessing = false;

                VisibilityThroughWindowAttribute attrVTW =
                    AttributeUtil.GetAttribute<VisibilityThroughWindowAttribute>(windowStackItem.type);
                if (attrVTW != null)
                {
                    switch (attrVTW.visibilityThroughWindowMode)
                    {
                        case VisibilityThroughWindowMode.NothingIsVisible:
                            if (coverSet == false)
                            {
                                coverSet = true;
                                windowCover.SetMode(true, false);
                                windowsCacheVisibleStack.Add(windowCover.transform);
                            }

                            // when we show fullscreen window, break stack interation - all other windows can be hidden
                            stopStackPrcessing = true;
                            break;
                        case VisibilityThroughWindowMode.TintedRaycastBlocker:
                            if (coverSet == false)
                            {
                                coverSet = true;
                                windowCover.SetMode(true, true);
                                windowsCacheVisibleStack.Add(windowCover.transform);
                            }

                            break;
                        case VisibilityThroughWindowMode.TransparentRaycastBlocker:
                            if (coverSet == false)
                            {
                                coverSet = true;
                                windowCover.SetMode(true, false);
                                windowsCacheVisibleStack.Add(windowCover.transform);
                            }

                            break;
                        case VisibilityThroughWindowMode.Hud:
                            hudExists = true;
                            // when we show hud window, break stack interation - no windows expect behind hud
                            stopStackPrcessing = true;
                            break;
                    }
                }

                if (stopStackPrcessing == true)
                {
                    break;
                }

                isFirstWindow = false;
            }

            IPromise allWindowsInstantiatePromise = Deferred.All(windowsInstantiatePromisesCache);
            windowsInstantiatePromisesCache.Clear();

            allWindowsInstantiatePromise.Done(() =>
            {
                //WARNING. A modal window trying to hide behind another modal window isn't supported by this logic.
                foreach (Transform w in windowsCacheVisibleStack)
                {
                    w.SetAsFirstSibling();
                }

                windowsCacheVisibleStack.Clear();
            });

            if (bgTypeRequired == BackgroundType.None && hudExists == false)
            {
                BackgroundManager.Set(BackgroundType.LoadingScreen);
            }
            else
            {
                BackgroundManager.Set(bgTypeRequired);
            }

            if (coverSet == false)
            {
                windowCover.SetMode(false, false);
            }

            // when show complete, hide all other windows
            for (int i = windowsCacheHideCandidates.Count - 1; i >= 0; i--)
            {
                transitionPromisesCache.Add(windowsCacheHideCandidates[i].HideFromUIManager());
            }

            windowsCacheHideCandidates.Clear();

            OnStackChanged();

            IPromise allTransitionsPromise = Deferred.All(transitionPromisesCache);
            transitionPromisesCache.Clear();

            return allTransitionsPromise;
        }

        #endregion windows background processes


        private void OnWindowCoverClick()
        {
            foreach (WindowStackItem windowStackItem in _windowsStack)
            {
                IWindow windowInfo = GetOrCreateWindowInstance(windowStackItem.type, out Exception exception);

                if (exception == null)
                {
                    windowInfo.OnWindowCoverClick();
                }

                break;
            }
        }

        public void Init()
        {
            Debug.LogWarning("UIManager Init is void");
        }

        public T WarmUp<T>() where T : class, IWindow
        {
            Exception exception;
            IWindow window = GetOrCreateWindowInstance(typeof(T), out exception);
            if (exception != null)
            {
                Debug.LogError(exception.Message);
            }

            return window as T;
        }

        public bool IsFullscreenWindowShown()
        {
            if (_windowsStack.Count == 0)
                return false;

            foreach (WindowStackItem windowStackItem in _windowsStack)
            {
                VisibilityThroughWindowAttribute attrVTW =
                    AttributeUtil.GetAttribute<VisibilityThroughWindowAttribute>(windowStackItem.type);
                if (attrVTW != null &&
                    attrVTW.visibilityThroughWindowMode == VisibilityThroughWindowMode.NothingIsVisible)
                    return true;
            }

            return false;
        }

        private List<RaycastResult> results = new List<RaycastResult>();

        public void IsPointHandledByUI(Vector2 screenPosition, out bool handlesClick, out bool handlesDrag)
        {
            if (IsFullscreenWindowShown())
            {
                handlesClick = true;
                handlesDrag = true;
                return;
            }

            handlesClick = false;
            handlesDrag = false;

            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = screenPosition;

            graphicRayСaster.Raycast(pointerEventData, results);
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.GetComponent<UnityEngine.EventSystems.IDragHandler>() != null)
                {
                    handlesClick = true;
                    handlesDrag = true;
                    break;
                }

                if (result.gameObject.GetComponent<IPointerClickHandler>() != null)
                {
                    handlesClick = true;
                }
            }

            results.Clear();
        }

        public IPromise<bool> ShowErrorMessage(ErrorType type, string message, string buttonActionText,
            string processMessage, string cancelButtonTextKey = null)
        {
            if (_lastShownErrorType.HasValue && _lastShownErrorType.Value == ErrorType.CriticalError)
            {
                return Deferred<bool>.GetFromPool().Resolve(false);
            }

            _lastShownErrorType = type;
            return criticalErrorPanel.Show(message, buttonActionText, processMessage, cancelButtonTextKey);
        }

        public void HideErrorMessage(ErrorType type)
        {
            if (_lastShownErrorType.HasValue && _lastShownErrorType.Value != type)
            {
                return;
            }

            criticalErrorPanel.Hide();
            _lastShownErrorType = null;
        }

        private class WindowStackItem
        {
            public Type type { get; protected set; }
            public Action<IWindow> initAction { get; protected set; }
        }

        private class WindowStackItem<T, D> : WindowStackItem where T : class, IWindow<D>
        {
            public WindowStackItem(D initData)
            {
                type = typeof(T);
                initAction = bw => (bw as T).Init(initData);
            }
        }
    }
}