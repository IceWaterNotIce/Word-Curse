using UnityEngine;
using System.Collections;
using System;
using System.IO;


public class BundleLoader : MonoBehaviour
{

    private IEnumerator coroutine;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }



    public void GetPrefabFromBundles<T>(string bundleName, string assetsName, Action<T> OnSuccess) where T : UnityEngine.Object
    {
        coroutine = LoadAssetBundles<T>(bundleName, assetsName, OnSuccess);
        StartCoroutine(coroutine);
    }

    IEnumerator LoadAssetBundles<T>(string bundleName, string assetsName, Action<T> OnSuccess) where T : UnityEngine.Object
    {
        string bundlePath = Path.Combine(Application.streamingAssetsPath, "Bundles", bundleName);
        Debug.Log(File.Exists(bundlePath));
        AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return bundleRequest;

        AssetBundle bundle = bundleRequest.assetBundle;
        if (bundle == null)
        {
            Debug.LogError("Failed to load AssetBundle: " + bundleName);

        }

        T prefab = bundle.LoadAsset<T>(assetsName);
        OnSuccess(prefab);
        bundle.Unload(false);
    }

}