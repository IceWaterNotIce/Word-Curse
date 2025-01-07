using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.PlayerLoop;
using InternetEmpire;

public class AssetBundleManager : MonoBehaviour
{
    private string localVersionPath;
    private string remoteVersionUrl;
    private string downloadPath;

    void Awake()
    {
        string platform = new string("");
        string platformFilePath = new string("");
        if (Application.platform == RuntimePlatform.Android)
        {
            platformFilePath = Path.Combine(Application.persistentDataPath, "platform.txt");
        }
        else
        {
            platformFilePath = Path.Combine(Application.streamingAssetsPath, "platform.txt");
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            // if file does not exist, create it
            if (!System.IO.File.Exists(platformFilePath))
            {
                System.IO.File.Create(platformFilePath).Close();
                System.IO.File.WriteAllText(platformFilePath, "android");
            }
        }

        // Check the local version config file exists
        if (System.IO.File.Exists(platformFilePath))
        {
            platform = System.IO.File.ReadAllText(platformFilePath);
        }

        remoteVersionUrl = "https://raw.githubusercontent.com/IceWaterNotIce/WordCurse/main/Assets/AssetBundles" + platform + "/version.json";

        if (Application.platform == RuntimePlatform.Android)
        {
            localVersionPath = Path.Combine(Application.persistentDataPath, "Bundles/version.json");
            downloadPath = Path.Combine(Application.persistentDataPath, "Bundles");
            
        }
        else
        {
            localVersionPath = Path.Combine(Application.streamingAssetsPath, "Bundles/version.json");
            downloadPath = Path.Combine(Application.streamingAssetsPath, "Bundles");
        }

    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return StartCoroutine(CheckAndUpdateBundles());
        // Load the lobby scene
        InitializeSceneManager initializeSceneManager = GameObject.FindFirstObjectByType<InitializeSceneManager>();
        initializeSceneManager.SetAssetBundleReady(true);
        Debug.Log("AssetBundleManager is ready");
    }

    IEnumerator CheckAndUpdateBundles()
    {
        // Load local version
        VersionConfig localConfig = new VersionConfig();


        if (File.Exists(localVersionPath))
        {
            string localJson = File.ReadAllText(localVersionPath);
            localConfig = JsonUtility.FromJson<VersionConfig>(localJson);
        }
        // Fetch remote version
        UnityWebRequest.ClearCookieCache();
        UnityWebRequest request = UnityWebRequest.Get(remoteVersionUrl);
        request.useHttpContinue = false;

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch remote version: " + request.error +
            "\n remoteVersionUrl: " + remoteVersionUrl);
            yield break;
        }

        Debug.Log(request.downloadHandler.text);

        var remoteConfig = JsonUtility.FromJson<VersionConfig>(request.downloadHandler.text);

        // Compare versions and download necessary bundles
        foreach (var remoteBundle in remoteConfig.bundles)
        {
            var localBundle = localConfig == null ? localConfig.bundles.Find(b => b.name == remoteBundle.name) : null;
            if (localBundle == null || float.Parse(remoteBundle.version) > float.Parse(localBundle.version))
            {
                yield return StartCoroutine(DownloadAssetBundle(remoteBundle.url, remoteBundle.name));
            }
        }

        // Update local version file
        File.WriteAllText(localVersionPath, request.downloadHandler.text);
    }

    IEnumerator DownloadAssetBundle(string url, string bundleName)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        string bundlePath = Path.Combine(downloadPath, bundleName);
        request.downloadHandler = new DownloadHandlerFile(bundlePath);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Failed to download {bundleName}: " + request.error);
        }
        else
        {
            Debug.Log($"{bundleName} downloaded successfully.");
        }
    }
}

[System.Serializable]
public class VersionConfig
{
    [System.Serializable]
    public class AssetBundleInfo
    {
        public string name;
        public string version;
        public string url;
    }
    public List<AssetBundleInfo> bundles;
}