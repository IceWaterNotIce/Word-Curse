using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class GameRoomSceneManager : MonoBehaviour
{
    [SerializeField] private WordJsonManager dataController;

    Word[] words;

    Word CurrentWord;

    [SerializeField] private TMP_Text TmpQuestion;
    [SerializeField] private TMP_InputField TmpInputAnswer;

    [SerializeField] private TMP_Text TmpMeaning;

    [SerializeField] private TMP_Text TmpNumTyped;

    [SerializeField] private TMP_Text TmpNumCorrect;

    [SerializeField] private TMP_Text TmpCorrectRate;
    // Update is called once per frame
    private InputSystem_Actions inputSystem;
      private void Awake()
    {
        inputSystem = new InputSystem_Actions();
        inputSystem.Enable();
        EnhancedTouchSupport.Enable();

    }
    async void Start()
    {
        await dataController.LoadFromCloud();
        dataController.SaveToLocal();
        words = dataController.GetWords();
        NewQuestion();
    }
    void Update()
    {
        if (inputSystem.UI.Submit.triggered && TmpInputAnswer.text != "")
        {
            CheckAnswer();
        }
    }

    public void GoToWordView()
    {
        SceneManager.LoadScene("WordView");
    }

    public void GoToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
    public void NewQuestion()
    {
        words = dataController.GetWords();
        CurrentWord = words[Random.Range(0, words.Length)];
        TmpQuestion.text = CurrentWord.word;
        TmpQuestion.gameObject.SetActive(false);
        TmpMeaning.text = CurrentWord.definition;

        TmpInputAnswer.text = "";
        TmpInputAnswer.Select();
        TmpInputAnswer.ActivateInputField();

        TmpNumTyped.text = "NumTyped : " + CurrentWord.numTyped.ToString();
        TmpNumCorrect.text = "NumCorrect : " + CurrentWord.numCorrect.ToString();

        //use bundleloader to load audio
        BundleLoader bundleLoader = FindFirstObjectByType<BundleLoader>();
        AudioSource audioSource = GetComponent<AudioSource>();
        bundleLoader.GetPrefabFromBundles("english_word.all", CurrentWord.word, (AudioClip audio) =>
        {
            audioSource.clip = audio;
            audioSource.Play();
        });
        //audioSource.clip = CurrentWord.audio;
        //audioSource.Play();

    }
    public void CheckAnswer()
    {
        //update numtyped and numcorrect
        CurrentWord.numTyped++;
        dataController.UpdateWord(CurrentWord);

        if (TmpInputAnswer.text.ToLower().Trim() == TmpQuestion.text.ToLower().Trim())
        {
            //update numcorrect
            CurrentWord.numCorrect++;
            dataController.UpdateWord(CurrentWord);
            Debug.Log("Correct");
            NewQuestion();

        }
        else
        {
            Debug.Log("Incorrect");
            ReTryQuestion();
        }


    }
    public void ReTryQuestion()
    {
        TmpQuestion.text = CurrentWord.word;
        TmpMeaning.text = CurrentWord.definition;

        TmpQuestion.gameObject.SetActive(true);

        TmpInputAnswer.text = "";
        TmpInputAnswer.Select();
        TmpInputAnswer.ActivateInputField();

        TmpNumTyped.text = "NumTyped : " + CurrentWord.numTyped.ToString();
        TmpNumCorrect.text = "NumCorrect : " + CurrentWord.numCorrect.ToString();
    }
}

