using Helpers.Promises;
using UnityEngine;

namespace Helpers.UI
{
    [RequireComponent(typeof(Camera))]
    public class CameraGrabber : MonoBehaviour
    {
        private Deferred deferred;
        private RenderTexture renderTexture;

        private void Awake()
        {
            enabled = false;
        }

        public IPromise GrabOne(ref RenderTexture renderTexture)
        {
            this.deferred = Deferred.GetFromPool();
            this.renderTexture = renderTexture;
            enabled = true;
            return deferred;
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (renderTexture != null && deferred != null)
            {
                Graphics.Blit(src, renderTexture);
                deferred.Resolve();
            }

            renderTexture = null;
            deferred = null;

            Graphics.Blit(src, dest);
            enabled = false;
        }
    }
}
