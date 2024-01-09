using Helpers.Promises;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Helpers.UI
{
    public class RawImagePlus : RawImage
    {
        [SerializeField] private float animationSpeed = 3f;

        private Deferred completePromise;

        private float currentVisibility = 1f;
        private float? animateTo;

        [Obsolete("use SetTexture() method", true)]
        [NonSerialized]
        new public Texture texture;

        [Obsolete("use SetTexture() method with empty texture to disable component", true)]
        [NonSerialized]
        new public bool enabled;

        [Obsolete("use Show() and Hide() methods", true)]
        [NonSerialized]
        new public Color color;

        private void Update()
        {
            if (animateTo.HasValue)
            {
                SetVisibility(Mathf.MoveTowards(currentVisibility, animateTo.Value, Time.unscaledDeltaTime * animationSpeed));

                if (Mathf.Approximately(currentVisibility, animateTo.Value))
                {
                    animateTo = null;

                    if (completePromise != null)
                    {
                        completePromise.Resolve();
                        completePromise = null;
                    }
                }
            }
        }

        public IPromise Show(Texture texture)
        {
            return ApplyTexture(texture);
        }

        public void ShowNow(Texture texture)
        {
            SetVisibility(1f);
            ApplyTexture(texture);
        }

        public IPromise Hide()
        {
            return AnimateAlphaTo(0f)
                .Done(()=>
                {
                    base.texture = null;
                    gameObject.SetActive(false);
                });
        }

        public void HideNow()
        {
            base.texture = null;
            gameObject.SetActive(false);
        }

        private IPromise ApplyTexture(Texture texture)
        {
            base.texture = texture;

            AspectRatioFitter aspectRatioFitter = GetComponent<AspectRatioFitter>() ?? gameObject.AddComponent<AspectRatioFitter>();
            aspectRatioFitter.aspectRatio = (float)texture.width / (float)texture.height;

            return AnimateAlphaTo(1f);
        }

        private IPromise AnimateAlphaTo(float alphaTarget)
        {
            if (completePromise == null)
            {
                completePromise = Deferred.GetFromPool();
            }

            animateTo = alphaTarget;
            gameObject.SetActive(true);

            return completePromise;
        }

        private void SetVisibility(float newVisibility)
        {
            currentVisibility = newVisibility;
            base.color = new Color(1f, 1f, 1f, currentVisibility);
        }
    }
}
