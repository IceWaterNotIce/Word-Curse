using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UIElements;


public class WordViewSceneUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    UIDocument m_UIDocument;
    VisualElement m_Root;

    public UnityEngine.UIElements.Button m_BtnGoToLobby;
    public VisualTreeAsset itemTemplate;
    public ListView m_LvWords;
    public TextField m_InputWord;
    public TextField m_InputDefinition;
    public DropdownField m_DdWordType;
    public UnityEngine.UIElements.Button m_BtnSubmit;
    public WordJsonManager wordJsonManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemTemplate = Resources.Load<VisualTreeAsset>("WordItemTemplate");
        // Get the UIDocument component
        m_UIDocument = GetComponent<UIDocument>();

        // Get the root of the VisualElement tree
        m_Root = m_UIDocument.rootVisualElement;
        // Get the GoToLobby button
        m_BtnGoToLobby = m_Root.Q<UnityEngine.UIElements.Button>("BtnGoToLobby");
        m_BtnGoToLobby.clicked += GoToLobby;
        // Get the ListView
        m_LvWords = m_Root.Q<ListView>("LvWords");
        m_LvWords.makeItem = () => itemTemplate.CloneTree();
        m_LvWords.bindItem = (e, i) =>
        {
            e.Q<Label>("LblWord").text = wordJsonManager.GetWords()[i].word;
            e.Q<Label>("LblDefinition").text = wordJsonManager.GetWords()[i].definition;
            e.Q<Label>("LblPartofSpeech").text = wordJsonManager.GetWords()[i].partofSpeech.ToString();
        };
        m_LvWords.fixedItemHeight = 50;
        m_LvWords.itemsSource = wordJsonManager.GetWords();

        m_LvWords.Rebuild();




        // Get the input fields
        m_InputWord = m_Root.Q<TextField>("InputWord");
        m_InputDefinition = m_Root.Q<TextField>("InputDefinition");
        // Get the submit button
        m_BtnSubmit = m_Root.Q<UnityEngine.UIElements.Button>("BtnSubmit");
        m_BtnSubmit.clicked += OnSubmit;
        // Get the WordType dropdown
        m_DdWordType = m_Root.Q<DropdownField>("DdWordType");
        List<string> options = new List<string>();
        foreach (PartofSpeech partofSpeech in (PartofSpeech[])System.Enum.GetValues(typeof(PartofSpeech)))
        {
            options.Add(partofSpeech.ToString());
        }
        m_DdWordType.choices = options;
    }
    async void OnEnable()
    {
        await wordJsonManager.LoadFromCloud();
        wordJsonManager.SaveToLocal();
        refreshList();

    }
    // Update is called once per frame
    void Update()
    {

    }

    public void refreshList()
    {
        m_LvWords.itemsSource = wordJsonManager.GetWords();
        m_LvWords.Rebuild();
    }

    public void GoToLobby()
    {
        // Load the Lobby scene
        SceneManager.LoadScene("Lobby");
    }

    public void OnSubmit()
    {
        wordJsonManager.InsertWord(m_InputWord.value, m_InputDefinition.value, (PartofSpeech)System.Enum.Parse(typeof(PartofSpeech), m_DdWordType.value));
        //clear the input fields
        m_InputWord.SetValueWithoutNotify("");
        m_InputDefinition.SetValueWithoutNotify("");
        m_DdWordType.value = m_DdWordType.choices[0];
    }
}
