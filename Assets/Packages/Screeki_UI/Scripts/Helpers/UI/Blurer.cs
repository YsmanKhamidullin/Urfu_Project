using UnityEngine;

namespace Helpers.UI
{
    public class Blurer : MonoBehaviour
    {
        [SerializeField] private Material blurMaterial;
        [SerializeField] private int blurIterations;

        private RenderTexture sourceRenderTexture;
        private RenderTexture tempRenderTexture;

        private int blurIterationsToDo;

        public void Blur(RenderTexture rt)
        {
            this.sourceRenderTexture = rt;

            if (tempRenderTexture == null || tempRenderTexture.width != rt.width || tempRenderTexture.height != rt.height)
            {
                this.tempRenderTexture = new RenderTexture(rt);
            }

            enabled = true;
            blurIterationsToDo = blurIterations;
        }

        private void Update()
        {
            if (blurIterationsToDo > 0)
            {
                Graphics.Blit(sourceRenderTexture, tempRenderTexture, blurMaterial);
                Graphics.Blit(tempRenderTexture, sourceRenderTexture, blurMaterial);
                tempRenderTexture.DiscardContents();
                blurIterationsToDo--;
            }
            else
            {
                enabled = false;
            }
        }
    }
}
