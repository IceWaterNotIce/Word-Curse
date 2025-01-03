using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Unity.VisualScripting;

namespace InternetEmpire
{
    public class MessageManager : Singleton<MessageManager>
    {
        [SerializeField] private GameObject Canvas;
        [SerializeField] private GameObject panelprefab;
        [SerializeField] private GameObject tmpTextPrefab;
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private GameObject buttonClosePrefab;

        void Start()
        {
        }

        public void CreateYesNoMessage(string message, System.Action onYes, System.Action onNo)
        {
            GameObject panel = Instantiate(panelprefab, Canvas.transform);
            GameObject tmpText = Instantiate(tmpTextPrefab, panel.transform);
            tmpText.GetComponent<TMP_Text>().text = message;
            GameObject buttonYes = Instantiate(buttonPrefab, panel.transform);
            buttonYes.GetComponentInChildren<TMP_Text>().text = "Yes";
            buttonYes.GetComponent<Button>().onClick.AddListener(() =>
            {
                onYes?.Invoke();
                Destroy(panel);
            });
            GameObject buttonNo = Instantiate(buttonPrefab, panel.transform);
            buttonNo.GetComponentInChildren<TMP_Text>().text = "No";
            buttonNo.GetComponent<Button>().onClick.AddListener(() =>
            {
                onNo?.Invoke();
                Destroy(panel);
            });
        }

        public void ToastMessage(string message, float duration = 2f)
        {
            GameObject panel = Instantiate(panelprefab, Canvas.transform);
            GameObject tmpText = Instantiate(tmpTextPrefab, panel.transform);
            tmpText.GetComponent<TMP_Text>().text = message;
            StartCoroutine(DestroyAfter(panel, duration));
        }

        private IEnumerator DestroyAfter(GameObject panel, float duration)
        {
            yield return new WaitForSeconds(duration);
            Destroy(panel);
        }

        public void CreateCloseMessage(string message, System.Action onClose)
        {
            GameObject panel = Instantiate(panelprefab, Canvas.transform);
            GameObject tmpText = Instantiate(tmpTextPrefab, panel.transform);
            tmpText.GetComponent<TMP_Text>().text = message;
            GameObject buttonClose = Instantiate(buttonClosePrefab, panel.transform);
            buttonClose.GetComponentInChildren<TMP_Text>().text = "Close";
            buttonClose.GetComponent<Button>().onClick.AddListener(() =>
            {
                onClose?.Invoke();
                Destroy(panel);
            });
        }
    }
}
