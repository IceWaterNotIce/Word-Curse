using UnityEngine;
using UnityEngine.SceneManagement;

public class WordViewSceneManager : MonoBehaviour
{

    public WordJsonManager wordJsonManager;
    public ScrollViewController scrollViewController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
     async void Start()
    {
        await wordJsonManager.LoadFromCloud();
        wordJsonManager.SaveToLocal();
        scrollViewController.LoadWords();
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
