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

    public GameObject SignUpPanel;

    public GameObject AuthPanel;
    public GameObject UserProfilePanel;

    [Header("UI Elements")]
    public TMP_Text TmpPlayerId;
    public TMP_Text TmpUsername;
    public TMP_Text TmpLastPasswordUpdate;
    public TMP_InputField SignInUsernameInput;
    public TMP_InputField SignInPasswordInput;

    public TMP_InputField SignUpUsernameInput;
    public TMP_InputField SignUpPasswordInput;
    void Start()
    {
        // show the canvas only on the right platform or in the editor
        runtimePlatform = Application.platform;
        gameObject.SetActive(runtimePlatform == ThisCanvasPlatform || runtimePlatform == RuntimePlatform.WindowsEditor);
    }

    void OnEnable()
    {

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
        AuthPanel.SetActive(!AuthenticationService.Instance.IsSignedIn);
        UserProfilePanel.SetActive(AuthenticationService.Instance.IsSignedIn);
        Debug.Log($"IsSignedIn: {AuthenticationService.Instance.IsSignedIn}");
        Debug.Log($"AuthPanel: {AuthPanel.activeSelf}");
        Debug.Log($"UserProfilePanel: {UserProfilePanel.activeSelf}");
    }

    public void BtnSignOutOnClick()
    {
        // sign out the user
        AuthenticationService.Instance.SignOut();
        // show sign in panel
        AuthPanel.SetActive(true);
        // hide user profile panel
        UserProfilePanel.SetActive(false);
    }

    public void GoToGraphView()
    {
        // Load the GraphView scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("GraphView");
    }

    public async void OnSignInBtnClick()
    {
        AuthManager authManager = FindFirstObjectByType<AuthManager>();
        await authManager.SignInWithUsernamePasswordAsync(SignInUsernameInput.text, SignInPasswordInput.text);
        UpdateUI();
        // if sign in is successful
        if (AuthenticationService.Instance.IsSignedIn)
        {
            SignInPanel.SetActive(false);
        }
    }

    public async void OnSignUpBtnClick()
    {
        AuthManager authManager = FindFirstObjectByType<AuthManager>();
        await authManager.SignUpWithUsernamePasswordAsync(SignUpUsernameInput.text, SignUpPasswordInput.text);
        UpdateUI();
        // if sign up is successful 
        if (AuthenticationService.Instance.IsSignedIn)
        {
            SignUpPanel.SetActive(false);
        }
    }

    public async void OnSignInAnonymouslyBtnClick()
    {
        AuthManager authManager = FindFirstObjectByType<AuthManager>();
        await authManager.SignInAnonymously();
        UpdateUI();
        // if sign in anonymously is successful
        if (AuthenticationService.Instance.IsSignedIn)
        {
            AuthPanel.SetActive(false);
        }
        
    }
    async void UpdateUI()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            TmpPlayerId.text = "Not signed in";
            TmpUsername.text = "Not signed in";
            TmpLastPasswordUpdate.text = "Not signed in";
            return;
        }

        PlayerInfo playerInfo = await AuthenticationService.Instance.GetPlayerInfoAsync();

        TmpPlayerId.text = playerInfo.Id ?? "Player ID not found";
        TmpUsername.text = playerInfo.Username ?? "Username not found";
        TmpLastPasswordUpdate.text = playerInfo.LastPasswordUpdate != null ? playerInfo.LastPasswordUpdate.Value.ToString() : "Password not updated";
    }
}