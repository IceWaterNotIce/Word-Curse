using UnityEngine;
using TMPro;

public class LobbySceneManager : MonoBehaviour
{

    [Header("UI Elements")]
    public TMP_Text TmpPlayerId;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToWordView()
    {
        // Load the WordView scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("WordView");
    }

    public void UpdatePlayerId()
    {
        // Update the PlayerId text with the current player id
        TmpPlayerId.text = $"Player ID: {AuthManager.Instance.GetPlayerId()}";
    }
}
