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
            versionCheckURL = "https://raw.githubusercontent.com/IceWaterNotIce/WordCurse/main/Assets/StreamingAssets/version.json";
            StartCoroutine(CheckForUpdate());
        }

        public IEnumerator CheckForUpdate()
        {
            UnityWebRequest www = UnityWebRequest.Get(versionCheckURL);
            yield return www.SendWebRequest();


            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Connect to server failed.");
                Debug.LogError(www.error);
            }
            else
            {
                // 解析 JSON 響應
                var jsonResponse = www.downloadHandler.text;
                versionInfo = JsonUtility.FromJson<VersionInfo>(jsonResponse);

                if (versionInfo.latestVersion != currentVersion)
                {
                    InitializeSceneManager initializeSceneManager = GameObject.FindFirstObjectByType<InitializeSceneManager>();
                    initializeSceneManager.SetVersionChecked(false);
                }
                else
                {
                    InitializeSceneManager initializeSceneManager = GameObject.FindFirstObjectByType<InitializeSceneManager>();
                    initializeSceneManager.SetVersionChecked(true);
                }
            }
        }

        [System.Serializable]
        public class VersionInfo
        {
            public string latestVersion;
            public string downloadURL;
        }

        
    }
}