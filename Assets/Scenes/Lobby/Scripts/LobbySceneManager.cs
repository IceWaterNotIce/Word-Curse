using UnityEngine;
using TMPro;

public class LobbySceneManager : MonoBehaviour
{

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

    public void GoToGameRoom()
    {
        // Load the GameRoom scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameRoom");
    }


}
