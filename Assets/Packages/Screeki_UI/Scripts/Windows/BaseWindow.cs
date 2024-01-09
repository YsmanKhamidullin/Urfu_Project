using System;
using Helpers.Promises;
using Helpers.UI;
using UnityEngine;

namespace Windows
{
    public abstract class BaseWindow : MonoBehaviour, Interfaces.IWindow
    {
        public enum WindowEventType
        {
            OnLoad,
            OnShowing,
            OnShown,
            OnHiding,
            OnHidden,
            OnUnload
        }

        public event Action<WindowEventType> OnWindowEvent = et => { };

        // used to hold (re)init commands until control is fully hidden and ready to update it's content
        // always resolved when control shown or showing
        // in pending mode during hiding animation
        protected IPromise actionsGate = Deferred.GetFromPool().Resolve();

        protected RectTransform windowRectTransform;

        public bool Initialized { get; private set; }

        public bool Shown { get; private set; }

        private WindowAnimator windowAnimator;

        private bool unloaded = false;

        private float oneTimeCloseDelay = 0f;

        private Func<IPromise> closeWindowRequest;

        protected void Start()
        {
            // don't use Start() in windows - use OnLoad
        }

        protected void Awake()
        {
            // don't use Awake() in windows - use OnLoad

            Initialized = false;

            windowRectTransform = GetComponent<RectTransform>();

            windowAnimator = GetComponent<WindowAnimator>() ?? gameObject.AddComponent<WindowAnimator>();

            if (windowAnimator == null)
            {
                windowAnimator = gameObject.AddComponent<WindowAnimator>();
            }
        }

        protected virtual void Update()
        {

        }

        protected void OnDestroy()
        {
            // don't use OnDestroy() in windows - use OnUnload

            if (unloaded == false)
            {
                Debug.LogWarning("windows is destroying externally without unloading using UI manager");
                OnUnload();
                unloaded = true;
            }
        }

        public Transform GetWindowRootTransform() { return unloaded ? null : transform; }

        public void SetOneTimeCloseDelay(float closeWindowDelay)
        {
            oneTimeCloseDelay = Mathf.Max(oneTimeCloseDelay, closeWindowDelay);
        }

        public void InitFromUIManager(Func<IPromise> closeWindowRequest)
        {
            this.closeWindowRequest = closeWindowRequest;
        }

        public IPromise SmoothDestroy()
        {
            if (this == null) { Deferred.GetFromPool().Resolve(); }

            return HideFromUIManager()
                .Done(() =>
                {
                    if (this != null)
                    {
                        OnUnload();
                        unloaded = true;
                        Destroy(gameObject);
                    }
                });
        }

        public IPromise ShowFromUIManager(bool thisWindowIsFirst, Action initAction)
        {
            if (Shown == false)
            {
                Shown = true;

                gameObject.SetActive(true);
                windowAnimator = GetComponent<WindowAnimator>() ?? gameObject.AddComponent<WindowAnimator>();

                actionsGate = actionsGate
                    .Then(() => Timers.Instance.WaitOneFrame()).Done(() => { if (initAction != null) { initAction(); } })
                    .Then(() => Timers.Instance.WaitOneFrame()).Done(() => { if (initAction != null) { Initialized = true; } })
                    .Then(() => Timers.Instance.WaitOneFrame()).Done(() => { if (initAction != null) { OnInit(); } })
                    .Then(() => Timers.Instance.WaitOneFrame()).Done(() => OnShowing())
                    .Then(() => Timers.Instance.WaitOneFrame()).Done(() => gameObject.SetActive(true))
                    .Then(() => Timers.Instance.WaitOneFrame()).Then(() => windowAnimator.Show())
                    .Then(() => Timers.Instance.WaitOneFrame()).Done(() => OnShown())
                    .Then(() => Timers.Instance.WaitOneFrame()).Done(() => { if (thisWindowIsFirst == true) { OnFocused(); } });
            }
            else
            {
                if (initAction != null)
                {
                    initAction();
                    OnSetInitDataWhileVisible();
                }
                if (thisWindowIsFirst == true)
                {
                    OnFocused();
                }
            }

            return actionsGate;
        }

        public IPromise HideFromUIManager()
        {
            if (Shown == true)
            {
                Shown = false;

                gameObject.SetActive(true);

                actionsGate = actionsGate
                    .Then(() => Timers.Instance.WaitOneFrame()).Done(() => gameObject.SetActive(true))
                    .Then(() => Timers.Instance.WaitUnscaled(oneTimeCloseDelay)).Done(() => oneTimeCloseDelay = 0f)
                    .Then(() => Timers.Instance.WaitOneFrame()).Done(() => OnHiding())
                    .Then(() => Timers.Instance.WaitOneFrame()).Then(() => windowAnimator.Hide())
                    .Then(() => Timers.Instance.WaitOneFrame()).Done(() => gameObject.SetActive(false))
                    .Then(() => Timers.Instance.WaitOneFrame()).Done(() => OnHidden());
            }

            return actionsGate;
        }

        public IPromise StackingHide()
        {
            return closeWindowRequest();
        }

        public virtual void OnWindowCoverClick() { }

        public virtual void HandleEscapeButton() { StackingHide(); }

        public virtual void OnLoad() { gameObject.SetActive(false); OnWindowEvent(WindowEventType.OnLoad); }

        public virtual void OnInit() { }

        public virtual void OnShowing() { OnWindowEvent(WindowEventType.OnShowing); }

        public virtual void OnShown() { OnWindowEvent(WindowEventType.OnShown); }

        public virtual void OnSetInitDataWhileVisible() { }

        public virtual void OnFocused() { }

        public virtual void OnHiding() { OnWindowEvent(WindowEventType.OnHiding); }

        public virtual void OnHidden() { OnWindowEvent(WindowEventType.OnHidden); }

        public virtual void OnUnload() { OnWindowEvent(WindowEventType.OnUnload); }

        /*
        Order of events:

                OnLoad()
                    ||
                    \/                on going forward
               SetInitData(T) <==============================================
                    ||                                                     ||
                    \/                                                     ||
                 OnInit()                                                  ||
                    ||                                                     ||
                    \/         on going backard                            ||
                OnShowing() <==============================================||
      (can call private SetupContent())                                    ||
                    ||                                                     ||
                    \/                                                     ||
                 OnShown()                                                 ||
                    ||                                                     ||
                    \/               on going backward wihout hide         ||
       OnSetInitDataWhileVisible() <=================================      ||
                    \/                                             ||      ||
                    ||                                             ||      ||
                OnFocused() =========================================      ||
                    ||                                                     ||
                    \/                                                     ||
                OnHiding()                                                 ||
                    ||                                                     ||
                    \/                                                     ||
                OnHidden() ==================================================
                    ||
                    \/
                OnUnload()
        */
    }

    public abstract class BaseWindow<T> : BaseWindow, Interfaces.IWindow<T>
    {
        protected T initData;

        public void Init(T initData) { this.initData = initData; }
    }
}
