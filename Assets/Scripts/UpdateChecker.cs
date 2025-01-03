using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


namespace InternetEmpire
{
    public class UpdateChecker : MonoBehaviour
    {
        private string versionCheckURL;
        private string currentVersion;

        public VersionInfo versionInfo;



        void Start()
        {
            currentVersion = Application.version;
            versionCheckURL = "https://raw.githubusercontent.com/IceWaterNotIce/Internet-Empire/main/Assets/StreamingAssets/version.json";
            StartCoroutine(CheckForUpdate());
        }

        public IEnumerator CheckForUpdate()
        {
            UnityWebRequest www = UnityWebRequest.Get(versionCheckURL);
            yield return www.SendWebRequest();


            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("錯誤: " + www.error);
                // 無法連接到伺服器，可能是網絡問題
                // 這裡可以提示用戶檢查網絡連接
                MessageManager.Instance.CreateYesNoMessage("Unable to connect to the server. Please check your network connection.", QuitGame, QuitGame);
            }
            else
            {
                // 解析 JSON 響應
                var jsonResponse = www.downloadHandler.text;
                versionInfo = JsonUtility.FromJson<VersionInfo>(jsonResponse);

                if (versionInfo.latestVersion != currentVersion)
                {
                    Debug.Log("有新版本可用: " + versionInfo.latestVersion);
                    // 可以提示用戶下載新版本
                    MessageManager.Instance.CreateYesNoMessage("There is a new version available. This game needs to run on the latest version. Do you want to update now?", UpdateGame, QuitGame);
                }
                else
                {
                    InitializeSceneManager initializeSceneManager = GameObject.FindFirstObjectByType<InitializeSceneManager>();
                    initializeSceneManager.isVersionChecked = true;
                    initializeSceneManager.Check();
                }
            }
        }

        [System.Serializable]
        public class VersionInfo
        {
            public string latestVersion;
            public string downloadURL;
        }

        public void UpdateGame()
        {
            Application.OpenURL(versionInfo.downloadURL);
            Application.Quit();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}