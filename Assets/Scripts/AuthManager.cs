using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using TMPro;

public class AuthManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text TmpPlayerId;

    public TMP_Text TmpUsername;
    public TMP_Text TmpLastPasswordUpdate;
    public TMP_InputField UsernameInput;
    public TMP_InputField PasswordInput;

    private PlayerInfo m_PlayerInfo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Awake()
    {
        //UnityServices.Initialize() will initialize all services that are subscribed to Core
        await UnityServices.InitializeAsync();
        Debug.Log($"Unity services initialization: {UnityServices.State}");

        //Shows if a cached session token exist
        Debug.Log($"Cached Session Token Exist: {AuthenticationService.Instance.SessionTokenExists}");

        // Shows Current profile
        Debug.Log($"Current Profile: {AuthenticationService.Instance.Profile}");
    }

    void Start()
    {

        if (AuthenticationService.Instance.SessionTokenExists)
        {
            SignInAnonymously();
        }
        if (AuthenticationService.Instance.IsSignedIn)
        {
            UpdateUI();
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

        m_PlayerInfo = await AuthenticationService.Instance.GetPlayerInfoAsync();

        TmpPlayerId.text = m_PlayerInfo.Id ?? "Player ID not found";
        TmpUsername.text = m_PlayerInfo.Username ?? "Username not found";
        TmpLastPasswordUpdate.text = m_PlayerInfo.LastPasswordUpdate != null ? m_PlayerInfo.LastPasswordUpdate.Value.ToString() : "Password not updated";
    }
    public async void SignUpWithUsernamePasswordAsync()
    {
        string username = UsernameInput.text;
        string password = PasswordInput.text;
        Debug.Log($"Username: {username}");
        Debug.Log($"Password: {password}");
        if (GameObject.Find("AuthenticatedGameObject") != null)
        {
            Debug.Log("Already signed in.");
            return;
        }
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            Debug.Log("SignUp With Username and PasswordAsync is successful.");

            GameObject AuthenticatedGameObject = new GameObject("AuthenticatedGameObject");
            DontDestroyOnLoad(AuthenticatedGameObject);
            UpdateUI();
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    public async void SignInWithUsernamePasswordAsync()
    {
        string username = UsernameInput.text;
        string password = PasswordInput.text;
        if (GameObject.Find("AuthenticatedGameObject") != null)
        {
            Debug.Log("Already signed in.");
            return;
        }
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            Debug.Log("ignIn With Username and Password is successful.");

            GameObject AuthenticatedGameObject = new GameObject("AuthenticatedGameObject");
            DontDestroyOnLoad(AuthenticatedGameObject);
            UpdateUI();
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    public async void AddUsernamePasswordAsync()
    {
        string username = UsernameInput.text;
        string password = PasswordInput.text;
        try
        {
            await AuthenticationService.Instance.AddUsernamePasswordAsync(username, password);
            Debug.Log("Username and password added.");
            UpdateUI();
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
    public async void SignInAnonymously()
    {
        if (AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("User is already signed in");
            return;
        }
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            UpdateUI();
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError($"Sign in anonymously failed with error code: {ex.ErrorCode}");
        }
    }

    public void SignOut()
    {
        AuthenticationService.Instance.SignOut();
        UpdateUI();
    }

    public void ClearSessionToken()
    {
        AuthenticationService.Instance.ClearSessionToken();
        UpdateUI();
    }
}
