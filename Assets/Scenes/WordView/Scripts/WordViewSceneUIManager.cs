using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
public class WordViewSceneUIManager : MonoBehaviour
{

    public RuntimePlatform ThisCanvasPlatform;
    private RuntimePlatform runtimePlatform;
    [Header("UI Elements")]
    public TMP_InputField m_wordInput;
    public TMP_InputField m_definitionInput;
    public TMP_Dropdown m_wordDropdown;
    public WordJsonManager wordJsonManager;
    public ScrollViewController scrollViewController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // show the canvas only on the right platform or in the editor
        runtimePlatform = Application.platform;
        gameObject.SetActive(runtimePlatform == ThisCanvasPlatform || runtimePlatform == RuntimePlatform.WindowsEditor);

        // Set the dropdown options based on the WordType enum
        m_wordDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (PartofSpeech partofSpeech in (PartofSpeech[])System.Enum.GetValues(typeof(PartofSpeech)))
        {
            options.Add(new TMP_Dropdown.OptionData(partofSpeech.ToString()));
        }
        m_wordDropdown.AddOptions(options);
    }
    async void OnEnable()
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
        wordJsonManager.InsertWord(m_wordInput.text, m_definitionInput.text, (PartofSpeech)m_wordDropdown.value);
    }
}
