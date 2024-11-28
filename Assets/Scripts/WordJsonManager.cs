using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using TMPro;
using Unity.Services.CloudSave;

public class Word
{
    public string word;
    public string definition;
}

public class WordJsonManager : MonoBehaviour
{
    Word[] m_words;

    [Header("UI Elements")]
    public TMP_InputField m_wordInput;
    public TMP_InputField m_definitionInput;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        //if file does not exist, create a new file
        if (!File.Exists(Path.Combine(Application.streamingAssetsPath, "words.json")))
        {
            File.Create(Path.Combine(Application.streamingAssetsPath, "words.json")).Close();
        }
        //wait for 100ms to make sure the cloud save service is initialized
        await Task.Delay(100);
        LoadFromCloud("words.json");
        SaveToLocal(m_words);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSubmit()
    {
        InsertWord(m_wordInput.text, m_definitionInput.text);
        Debug.Log(m_words[m_words.Length - 1].word + ", " + m_words[m_words.Length - 1].definition);
    }

    public void InsertWord(string word, string definition)
    {
        Word newWord = new Word();
        newWord.word = word;
        newWord.definition = definition;
        List<Word> wordList = new List<Word>();
        if (m_words != null)
        {
            wordList.AddRange(m_words);
        }

        wordList.Add(newWord);
        m_words = wordList.ToArray();

        SaveToLocal(m_words);
        SaveToCloud("words.json");

        Debug.Log("Inserted " + word);
    }

    public void DeleteWord(string word)
    {
        if (m_words == null)
        {
            Debug.LogError("Words array is null!");
            return;
        }
        List<Word> wordList = new List<Word>(m_words);
        wordList.RemoveAll(w => w.word == word);
        m_words = wordList.ToArray();

        SaveToLocal(m_words);
        SaveToCloud("words.json");

        Debug.Log("Deleted " + word);
    }

    public void SaveToLocal(Word[] words)
    {
        string json = JsonConvert.SerializeObject(words);
        string filePath = Path.Combine(Application.streamingAssetsPath, "words.json");
        File.WriteAllText(filePath, json);
    }


    public void SaveToCloud(string fileName)
    {
        // get word json from streaming assets
        string filePath = Path.Combine(Application.streamingAssetsPath, "words.json");

        byte[] jsonData = File.ReadAllBytes(filePath);

        // save word json to cloud save
        CloudSaveManager.Instance.SavePlayerFile(fileName, jsonData);
    }

    public async void LoadFromCloud(string fileName)
    {
        byte[] fileBytes = await CloudSaveManager.Instance.LoadPlayerFile(fileName);
        if (fileBytes == null)
        {
            SaveToCloud("words.json");
            LoadFromCloud("words.json");
            return;
        }
        string jsonString = Encoding.UTF8.GetString(fileBytes);
        Word[] words = JsonConvert.DeserializeObject<Word[]>(jsonString);
        m_words = words;
    }

    void OnDestory()
    {
        SaveToLocal(m_words);
        SaveToCloud("words.json");
    }
}
