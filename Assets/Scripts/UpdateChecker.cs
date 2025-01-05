using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Windows;
using System.IO;


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

                string platform = new string("");
#if UNITY_ANDROID
            string platformFilePath = Path.Combine(Application.persistentDataPath, "Bundles/version.json");
#else
                string platformFilePath = Path.Combine(Application.streamingAssetsPath, "Bundles/version.json");
#endif

#if UNITY_ANDROID
            UnityWebRequest localRequest = UnityWebRequest.Get(localVersionPath);
            yield return localRequest.SendWebRequest();
            if (localRequest.result == UnityWebRequest.Result.Success)
            {
                platform = localRequest.downloadHandler.text;
            }
            else
            {
                Debug.Log("local version.json not exist.");
                yield break;
            }
#else
                // Check the local version config file exists
                if (System.IO.File.Exists(platformFilePath))
                {
                    platform = System.IO.File.ReadAllText(platformFilePath);
                }
#endif

                foreach (VersionConfig.VersionInfo info in versionData.platforms)
                {
                    // Debug.Log("info.name: " + info.name);
                    // Debug.Log("platform: " + PlayerPrefs.GetString("platform"));
                    // Debug.Log(info.name == PlayerPrefs.GetString("platform"));
                    if (info.name == platform)
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
                    }
                }
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