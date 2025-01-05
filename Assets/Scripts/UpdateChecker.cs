using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;


namespace InternetEmpire
{
    public class UpdateChecker : MonoBehaviour
    {
        private string versionCheckURL;
        private string currentVersion;

        public VersionConfig.VersionInfo versionInfo;



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
                VersionConfig versionData = JsonUtility.FromJson<VersionConfig>(jsonResponse);

                foreach (VersionConfig.VersionInfo info in versionData.platforms)
                {
                    if (info.name == Application.platform.ToString())
                    {
                        versionInfo = info;
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
                        break;
                    }
                }
                MessageManager messageManager = GameObject.FindFirstObjectByType<MessageManager>();
                messageManager.CreateCloseMessage("Current application platform: " + Application.platform, null);
                Debug.Log("Current application platform: " + Application.platform);
                Debug.LogError("No version info found for current platform.");
            }
        }

        [System.Serializable]
        public class VersionConfig
        {
            [System.Serializable]
            public class VersionInfo
            {
                public string name;
                public string latestVersion;
                public string downloadURL;
            }
            public List<VersionInfo> platforms;
        }


    }
}