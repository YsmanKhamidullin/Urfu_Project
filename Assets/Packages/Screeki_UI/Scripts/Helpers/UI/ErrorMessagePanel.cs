using Helpers.Promises;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Helpers.UI
{
    public class ErrorMessagePanel : MonoBehaviour
    {
        [SerializeField] private Button actionButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private RectTransform processIcon;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private TextMeshProUGUI buttonActionText;
        [SerializeField] private TextMeshProUGUI cancelText;

        private Deferred<bool> deferred;
        private string processMessage;

        private void Awake()
        {
            actionButton.onClick.AddListener(OnActionClick);
            cancelButton.onClick.AddListener(OnCancelClick);
            gameObject.SetActive(false);
        }

        private void OnCancelClick()
        {
            if (deferred != null)
            {
                actionButton.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
                deferred.Resolve(false);
                deferred = null;
            }
        }

        public IPromise<bool> Show(string message, string buttonActionText, string processMessage, string cancelText = null)
        {
            this.buttonActionText.text = buttonActionText;
            actionButton.gameObject.SetActive(true);
            this.cancelText.text = cancelText;
            cancelButton.gameObject.SetActive(string.IsNullOrEmpty(cancelText) == false);
            messageText.text = message;
            gameObject.SetActive(true);
            deferred = Deferred<bool>.GetFromPool();
            processIcon.gameObject.SetActive(false);
            this.processMessage = processMessage;

            return deferred;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }



        private void Update()
        {
            if (processIcon != null && processIcon.gameObject.activeSelf)
            {
                processIcon.Rotate(Vector3.forward * Time.unscaledDeltaTime * -360);
            }
            if (cancelButton.gameObject.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Escape) == true)
                {
                    OnCancelClick();
                }
            }
        }

        private void OnActionClick()
        {
            if (deferred != null)
            {
                messageText.text = processMessage;
                processIcon.gameObject.SetActive(true);
                actionButton.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
                deferred.Resolve(true);
                deferred = null;
            }
        }
    }
}
