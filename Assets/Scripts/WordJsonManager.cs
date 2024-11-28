using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using TMPro;

public class Word
{
    public string word;
    public string definition;

    public int numTyped;
    public int numCorrect;
    
}

public class WordJsonManager : MonoBehaviour
{
    private Word[] m_words;
    public Word[] GetWords()
    {
        
        return m_words;
    }


    static string m_fileName = "words.json";
    string m_filePath;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set the file path based on the platform
        if (Application.platform == RuntimePlatform.Android)
        {
            m_filePath = Path.Combine(Application.persistentDataPath, m_fileName);
        }
        else
        {
            m_filePath = Path.Combine(Application.streamingAssetsPath, m_fileName);
        }


    }

    // Update is called once per frame
    void Update()
    {

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

        SaveToLocal();
        SaveToCloud();

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

        SaveToLocal();
        SaveToCloud();

        Debug.Log("Deleted " + word);
    }

    public void LoadFromLocal()
    {
        // if environment is web, return
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            return;
        }
        // if file does not exist, create it
        if (!File.Exists(m_filePath))
        {
            File.Create(m_filePath).Close();
        }

        string filePath = m_filePath;
        string json = File.ReadAllText(filePath);
        Word[] words = JsonConvert.DeserializeObject<Word[]>(json);
        m_words = words;
    }
    public void SaveToLocal()
    {
        // if environment is web, return
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            return;
        }
        string json = JsonConvert.SerializeObject(m_words);
        string filePath = m_filePath;
        File.WriteAllText(filePath, json);
    }

    public void SaveToCloud()
    {
        string jsonData = JsonConvert.SerializeObject(m_words);
        byte[] fileBytes = Encoding.UTF8.GetBytes(jsonData);

        // save word json to cloud save
        CloudSaveManager.Instance.SavePlayerFile(m_fileName, fileBytes);
    }
    public async Task LoadFromCloud()
    {
        byte[] fileBytes = await CloudSaveManager.Instance.LoadPlayerFile(m_fileName);
        if (fileBytes == null)
        {
            SaveToCloud();
            await LoadFromCloud();
            return;
        }
        string jsonString = Encoding.UTF8.GetString(fileBytes);
        Word[] words = JsonConvert.DeserializeObject<Word[]>(jsonString);
        m_words = words;
    }

    void OnDestory()
    {
        SaveToLocal();
        SaveToCloud();
    }

    public void UpdateWord(Word word)
    {
        if (m_words == null)
        {
            Debug.LogError("Words array is null!");
            return;
        }
        List<Word> wordList = new List<Word>(m_words);
        int index = wordList.FindIndex(w => w.word == word.word);
        if (index == -1)
        {
            Debug.LogError("Word not found!");
            return;
        }
        wordList[index] = word;
        m_words = wordList.ToArray();

        SaveToLocal();
        SaveToCloud();

        Debug.Log("Updated " + word.word);
    }
}
