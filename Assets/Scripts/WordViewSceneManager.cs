using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WordViewSceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField m_wordInput;
    public TMP_InputField m_definitionInput;
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

    public void OnSubmit()
    {
        wordJsonManager.InsertWord(m_wordInput.text, m_definitionInput.text);
    }
}
