using Helpers.Promises;
using UnityEngine;

namespace Helpers.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class WindowAnimator : MonoBehaviour
    {
        private struct State
        {
            public float alpha;
            public float scale;
        }

        [SerializeField] private bool animateScale = true;
        [SerializeField] private bool animateAlpha = true;

        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;

        private State visibleState = new State { alpha = 1f, scale = 1f };
        private State hiddenState = new State { alpha = 0f, scale = 1.2f };

        private float currentState = 0f;
        private float goalState = 0f;
        private const float animationSpeed = 7.5f;

        private Deferred onCompleteDeferred;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            ApplyState();
        }

        public IPromise Show()
        {
            ResolveAndStop(true);

            enabled = true;
            onCompleteDeferred = Deferred.GetFromPool();
            goalState = 1f;

            return onCompleteDeferred;
        }

        public IPromise Hide()
        {
            ResolveAndStop(true);

            enabled = true;
            onCompleteDeferred = Deferred.GetFromPool();
            goalState = 0f;

            return onCompleteDeferred;
        }

        public bool IsHidden { get { return goalState < 0.001f; } }

        private void Update()
        {
            if (Mathf.Approximately(currentState, goalState) == true)
            {
                ResolveAndStop(false);
            }
            else
            {
                currentState = Mathf.MoveTowards(currentState, goalState, Time.unscaledDeltaTime * animationSpeed);
                ApplyState();
            }
        }

        private void ResolveAndStop(bool interrupted)
        {
            enabled = false;

            Deferred deferred = onCompleteDeferred;
            onCompleteDeferred = null;

            if (deferred != null)
            {
                if (interrupted)
                {
                    deferred.Reject(new System.Exception("Animation interrupted by another command"));
                }
                else
                {
                    deferred.Resolve();
                }
            }
        }

        private void ApplyState()
        {
            State state = GetCurrentState();
            if(animateAlpha == true)
            {
                canvasGroup.alpha = state.alpha;
            }
            if (animateScale == true)
            {
                rectTransform.localScale = Vector3.one * state.scale;
            }
        }

        private State GetCurrentState()
        {
            return new State
            {
                alpha = Mathf.Lerp(hiddenState.alpha, visibleState.alpha, currentState),
                scale = Mathf.Lerp(hiddenState.scale, visibleState.scale, currentState),
            };
        }
    }
}
