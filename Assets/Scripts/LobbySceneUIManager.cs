using System.Collections.Generic;
using UnityEngine;

public class LobbySceneUIManager : MonoBehaviour
{
    private RuntimePlatform runtimePlatform;


    [Header("Canvases")]
    public Canvas Android;
    public Canvas Windows;

    void Start()
    {
        
    }

    void OnEnable()
    {
        runtimePlatform = Application.platform;
        if (runtimePlatform == RuntimePlatform.Android)
        {
            Android.gameObject.SetActive(true);
            Windows.gameObject.SetActive(false);
        }
        else if (runtimePlatform == RuntimePlatform.WindowsEditor)
        {
            Android.gameObject.SetActive(false);
            Windows.gameObject.SetActive(true);
        }
    }
}