using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class ScrollViewController : MonoBehaviour
{
    public GameObject wordItemPrefab;
    public Transform contentTransform;

    public WordJsonManager wordJsonManager;

    void Start()
    {

    }

    public void LoadWords()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        Word[] words = wordJsonManager.GetWords();
        if (words == null)
        {
            return;
        }
        for (int i = 0; i < words.Length; i++)
        {
            var wordItem = Instantiate(wordItemPrefab, contentTransform).transform;
            var TmpWord = wordItem.GetChild(0);
            TmpWord.GetComponent<TextMeshProUGUI>().text = words[i].word;
            var TmpMeaning = wordItem.GetChild(1);
            TmpMeaning.GetComponent<TextMeshProUGUI>().text = words[i].definition;
            // Add a button to delete the word
            var deleteButton = wordItem.GetChild(2);
            var word = words[i].word;
            deleteButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                wordJsonManager.DeleteWord(word);
                LoadWords();
            });
        }

        contentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, wordItemPrefab.GetComponent<RectTransform>().rect.height * words.Length);
    }
}