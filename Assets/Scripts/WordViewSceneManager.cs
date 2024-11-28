using UnityEngine;
using UnityEngine.SceneManagement;

public class WordViewSceneManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GoToLobby()
    {
        // Load the Lobby scene
        SceneManager.LoadScene("Lobby");
    }
}
