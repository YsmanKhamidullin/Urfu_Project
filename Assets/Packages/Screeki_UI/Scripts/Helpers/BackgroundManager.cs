using Helpers.UI;
using Helpers.Promises;
using UnityEngine;

namespace Helpers
{
    public class BackgroundManager : MonoBehaviour
    {
        public bool BackgroundVisible { get; private set; }

        [SerializeField] private RawImagePlus textureHolder;
        [SerializeField] private Texture2D[] splashScreens;
        [SerializeField] private Texture2D blurredDarkTexture;
        [SerializeField] private new Camera camera;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Blurer blurer;

        private static BackgroundManager instance;

        private RenderTexture cameraView;
        private Camera grabbedCamera;

        private BackgroundType transitionEndType;
        private IPromise transitionPromise = Deferred.GetFromPool().Resolve();

        public static BackgroundManager GetInstance()
        {
            if (instance == null)
            {
                instance = Instantiate(Resources.Load<BackgroundManager>("BackgroundManager"));
            }
            return instance;
        }

        public static bool InstanceExists()
        {
            return instance != null;
        }

        public static void Load()
        {
            GetInstance();
        }

        public static void Set(BackgroundType backgroundType)
        {
            GetInstance().SetInternal(backgroundType);
        }

        public static void SetVisible(bool visible)
        {
            GetInstance().SetVisibleInternal(visible);
        }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            // show black wall
            BackgroundCameraOn(null);

            // show splash immediately
            textureHolder.ShowNow(RollSplashScreen());
        }

        private IPromise SetInternal(BackgroundType requiredBackgroundType)
        {
            if (requiredBackgroundType != transitionEndType)
            {
                switch (requiredBackgroundType)
                {
                    case BackgroundType.None:
                        transitionPromise = transitionPromise.Then(GoBackgroundNone);
                        break;
                    case BackgroundType.LoadingScreen:
                        transitionPromise = transitionPromise.Then(GoBackgroundLoadingScreen);
                        break;
                    case BackgroundType.StaticBlurredDark:
                        transitionPromise = transitionPromise.Then(GoBackgroundStaticBlurredDark);
                        break;
                    case BackgroundType.BlurredCameraView:
                        transitionPromise = transitionPromise.Then(GoBackgroundBlurredCameraView);
                        break;
                    default:
                        Debug.LogError("unknown background type: " + requiredBackgroundType);
                        break;
                }

                transitionEndType = requiredBackgroundType;
            }

            return transitionPromise;
        }

        private IPromise GoBackgroundNone()
        {
            FreeUpCamera();
            return textureHolder.Hide();
        }

        private IPromise GoBackgroundLoadingScreen()
        {
            return textureHolder.Show(RollSplashScreen()).Done(GrabCamera);
        }

        private IPromise GoBackgroundStaticBlurredDark()
        {
            return textureHolder.Show(blurredDarkTexture).Done(GrabCamera);
        }

        private IPromise GoBackgroundBlurredCameraView()
        {
            GrabCamera();
            textureHolder.ShowNow(cameraView);
            // don't wait for blurer - continue process
            return Deferred.GetFromPool().Resolve();
        }

        private void SetVisibleInternal(bool visible)
        {
            gameObject.SetActive(visible == true);
        }

        private void GrabCamera()
        {
            if (cameraView == null)
            {
                cameraView = new RenderTexture(Screen.width, Screen.height, 16);
                cameraView.name = "camera view render texture";
                cameraView.MarkRestoreExpected();
            }

            if (grabbedCamera == null)
            {
                grabbedCamera = Camera.main;

                if (grabbedCamera == null)
                {
                    BackgroundCameraOn(null);
                }
                else
                {
                    CameraGrabber cameraGrabber = grabbedCamera.GetComponent<CameraGrabber>() ?? grabbedCamera.gameObject.AddComponent<CameraGrabber>();

                    cameraGrabber.GrabOne(ref cameraView)
                        .Done(() =>
                        {
                            if (grabbedCamera != null)
                            {
                                BackgroundCameraOn(grabbedCamera.gameObject);
                            }
                            blurer.Blur(cameraView);
                        });
                }
            }
        }

        private void FreeUpCamera()
        {
            BackgroundCameraOff(grabbedCamera == null ? null : grabbedCamera.gameObject);
            grabbedCamera = null;
        }

        private IPromise onOffCycle = Deferred.GetFromPool().Resolve();

        private void BackgroundCameraOn(GameObject sceneCameraToDisable)
        {
            onOffCycle = onOffCycle.Then(() =>
            {
                camera.backgroundColor = Color.black;
                camera.clearFlags = CameraClearFlags.Color;
                camera.enabled = true;

                return Timers.Instance.WaitOneFrame()
                    .Done(() =>
                    {
                        canvas.renderMode = RenderMode.ScreenSpaceCamera;
                        canvas.worldCamera = camera;
                    })
                    .Then(Timers.Instance.WaitOneFrame)
                    .Done(() =>
                    {
                        if (sceneCameraToDisable != null)
                        {
                            sceneCameraToDisable.SetActive(false);
                        }
                    });
            });
        }

        private void BackgroundCameraOff(GameObject sceneCameraToEnable)
        {
            onOffCycle = onOffCycle.Then(() =>
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.worldCamera = null;

                return Timers.Instance.WaitOneFrame()
                    .Done(() =>
                    {
                        if (sceneCameraToEnable != null)
                        {
                            sceneCameraToEnable.SetActive(true);
                        }
                    })
                    .Then(Timers.Instance.WaitOneFrame)
                    .Done(() =>
                     {
                         camera.enabled = false;
                     });
            });
        }

        private Texture2D RollSplashScreen()
        {
            if (splashScreens != null && splashScreens.Length > 0)
            {
                return splashScreens[Random.Range(0, splashScreens.Length)];
            }

            return null;
        }
    }

    public enum BackgroundType : byte
    {
        None = 0,
        LoadingScreen = 1,
        StaticBlurredDark = 2,
        BlurredCameraView = 250,
    }
}
