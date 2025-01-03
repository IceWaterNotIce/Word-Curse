using UnityEngine;
using Unity.Services.Authentication;
using System.Collections.Generic;
using TMPro;
public class LobbySceneUIManager : MonoBehaviour
{
    public RuntimePlatform ThisCanvasPlatform;
    private RuntimePlatform runtimePlatform;

    [Header("Panels")]

    public GameObject SignInPanel;
    public GameObject UserProfilePanel;

    [Header("UI Elements")]
    public TMP_Text TmpPlayerId;
    public TMP_Text TmpUsername;
    public TMP_Text TmpLastPasswordUpdate;
    public TMP_InputField UsernameInput;
    public TMP_InputField PasswordInput;
    void Start()
    {
        // show the canvas only on the right platform or in the editor
        runtimePlatform = Application.platform;
        gameObject.SetActive(runtimePlatform == ThisCanvasPlatform || runtimePlatform == RuntimePlatform.WindowsEditor);
    }

    void OnEnable()
    {


        // show the sign in panel if the user is not signed in
        if (runtimePlatform == ThisCanvasPlatform)
        {
            AuthManager authManager = FindFirstObjectByType<AuthManager>();
            authManager.TmpPlayerId = TmpPlayerId;
            authManager.TmpUsername = TmpUsername;
            authManager.TmpLastPasswordUpdate = TmpLastPasswordUpdate;
            authManager.UsernameInput = UsernameInput;
            authManager.PasswordInput = PasswordInput;
        }

    }

    public void SwitchActiveState(GameObject obj)
    {
        // Close the panel
        obj.SetActive(!obj.activeSelf);
    }

    public void BtnAuthOnClick()
    {
        // if user is not signed in, show sign in panel
        // if user is signed in, show user profile panel
        SignInPanel.SetActive(!AuthenticationService.Instance.IsSignedIn);
        UserProfilePanel.SetActive(AuthenticationService.Instance.IsSignedIn);
    }

    public void BtnSignOutOnClick()
    {
        // sign out the user
        AuthenticationService.Instance.SignOut();
        // show sign in panel
        SignInPanel.SetActive(true);
        // hide user profile panel
        UserProfilePanel.SetActive(false);
    }



}