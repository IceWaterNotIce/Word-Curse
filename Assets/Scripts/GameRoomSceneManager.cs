using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

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

    async void Start()
    {
        await dataController.LoadFromCloud();
        dataController.SaveToLocal();
        words = dataController.GetWords();
        NewQuestion();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && TmpInputAnswer.text != "")
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

