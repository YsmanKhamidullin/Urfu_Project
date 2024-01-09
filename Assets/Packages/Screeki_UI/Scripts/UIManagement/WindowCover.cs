using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement
{
    public class WindowCover : MonoBehaviour
    {
        public event Action OnCoverClick = () => { };

        [SerializeField] private Graphic coverImageTinted;
        [SerializeField] private Graphic coverImageTransparent;

        private void Awake()
        {
            if (coverImageTinted.GetComponent<Button>())
            {
                coverImageTinted.GetComponent<Button>().onClick.AddListener(() => { OnCoverClick(); });
            }
            if (coverImageTransparent.GetComponent<Button>())
            {
                coverImageTransparent.GetComponent<Button>().onClick.AddListener(() => { OnCoverClick(); });
            }

            coverImageTinted.raycastTarget = true;
            coverImageTransparent.raycastTarget = true;
        }

        public void SetMode(bool enabled, bool showTint)
        {
            gameObject.SetActive(enabled);

            if (enabled == true)
            {
                coverImageTinted.enabled = showTint == true;
                coverImageTransparent.enabled = showTint == false;
            }
        }
    }
}
