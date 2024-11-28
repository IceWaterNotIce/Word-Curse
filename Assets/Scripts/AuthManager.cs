using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class AuthManager : Singleton<AuthManager>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override async void Awake()
    {
        base.Awake();

        //UnityServices.Initialize() will initialize all services that are subscribed to Core
        await UnityServices.InitializeAsync();
        Debug.Log($"Unity services initialization: {UnityServices.State}");

        //Shows if a cached session token exist
        Debug.Log($"Cached Session Token Exist: {AuthenticationService.Instance.SessionTokenExists}");

        // Shows Current profile
        Debug.Log($"Current Profile: {AuthenticationService.Instance.Profile}");
    }

    // Update is called once per frame
    void Update()
    {

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
            SuccessSignIn();
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError($"Sign in anonymously failed with error code: {ex.ErrorCode}");
        }
    }

    void SuccessSignIn()
    {
        Debug.Log("Sign in successful");
        Debug.Log($"PlayedID: {AuthenticationService.Instance.PlayerId}");
    }

    public string GetPlayerId()
    {
        return AuthenticationService.Instance.PlayerId;
    }

    public void SignOut()
    {
        AuthenticationService.Instance.SignOut();
    }

    public void ClearSessionToken()
    {
        AuthenticationService.Instance.ClearSessionToken();
    }
}
