using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Papae.UnitySDK.DesignPatterns;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject _letterButtonPrefab;
    //[SerializeField] GameObjectPoolBehaviour poolHebaviour;
    [SerializeField] WordManager _wordController;
    [SerializeField] LetterRack _playRack;
    [SerializeField] LetterRack _handRack;
    [SerializeField] bool _debugMode;

    [Header("Counters")]
    [SerializeField] Score _currentScore;
    [SerializeField] Timer _currentTimer;
    [SerializeField] Tally _currentRepairRatio;

    [Header("User Interface")]
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] Image _progessDisplay;

    [SerializeField] TextMeshProUGUI _pidginText;

    //private PersistentDataManagerScript dataManager;
    private Hashtable data;

    private void Start()
    {
        LoadData();
        Initialise();
        SetupGame();
    }
    
   void LoadData()
    {
        if (data == null || _debugMode)
        {
            _wordController.Setup();
        }
        else
        {
            _wordController.Setup(data);
        }
    }

    void Initialise()
    {
        _currentScore.Reset();
        _currentTimer.Reset();

        _playRack.SpawnSlots(GetRepairWord().Length);
        _handRack.SpawnSlots(GetRepairWord().Length);
    }

    string GetCurrentPidgenWord()
    {
        return _wordController.GetCurrentWord();
    }

    string GetRepairWord()
    {
        return (string)_wordController.unsolvedWords[GetCurrentPidgenWord()];
    }

    void SetupGame()
    {
        //Clear racksf this isn't a new game
        _playRack.ClearRack();
        _handRack.ClearRack();

        string shuffledWord = _wordController.ShuffleWord(GetRepairWord());

        _pidginText.text = GetCurrentPidgenWord();

        //Create tiles d add them to Rack
        foreach (char c in shuffledWord)
        {
            GameObject clone = (GameObject)Instantiate(_letterButtonPrefab, Vector3.zero, transform.rotation);
            //GameObject clone = poolHebaviour.RequestGameObject();
            clone.name = c.ToString();

            LetterButton letterButton = clone.GetComponent<LetterButton>();
            letterButton.Letter = c.ToString();
            
            //TODO: Maybe directly add these tiles so it doesn't look visually weird
            _handRack.AddLetterButtonToFirstEmptySlot(letterButton);
        }

        _currentTimer.Begin();
        SetProgressRatio();
    }

    public void SetProgressRatio()
    {
        float percentage = 0;
        try
        {
            percentage= GetNumSolvedWords() / (GetNumSolvedWords() + GetNumUnsolvedWords());
        }
        catch (Exception ex)
        {
            
        }
        _progessDisplay.fillMethod = Image.FillMethod.Horizontal;
        _progessDisplay.fillAmount = percentage;
    }

    public void UpdateScore(int value)
    {
        _currentScore.CurrentValue += value;
        _scoreText.text = _currentScore.CurrentValue.ToString();
    }

    void Update()
    {
        if (Input.GetKey("c"))
        {
            //dataManager.Clear();
        }

        _timerText.text = ((int)_currentTimer.CurrentValueInSeconds).ToString();


        if (_playRack.GetRackString() == GetRepairWord())
        {
            //Game is won
            //Get new word. Should maybe just leave it all in the word manager?
            _wordController.MarkWordAsSolved(GetCurrentPidgenWord());
            _currentScore.CurrentValue += 5;

            if (GetNumUnsolvedWords() == 0)
            {

            }
            else
            {
                _wordController.SetCurrentWord(_wordController.GetUnsolvedWord());
                SaveProgress();
                SetupGame();
            }
        }
    }

    public void ShuffleWord()
    {
        _handRack.Shuffle();
    }

    public void SkipWord()
    {
        _wordController.SetCurrentWord(_wordController.GetUnsolvedWord());
        SaveProgress();
        SetupGame();
    }

    public void HintWord()
    {
        _currentTimer.CurrentValueInSeconds -= 10f; 
        _wordController.SetCurrentWord(_wordController.GetUnsolvedWord());
        SaveProgress();
        SetupGame();
    }

    public int GetNumSolvedWords()
    {
        return _wordController.GetNumSolvedWords();
    }

    public int GetNumUnsolvedWords()
    {
        return _wordController.GetNumUnsolvedWords();
    }

    private void SaveProgress()
    {
        //Save Progress
        Hashtable progress = new Hashtable();
        progress.Add("solvedWords", _wordController.solvedWords);
        progress.Add("unsolvedWords", _wordController.unsolvedWords);
        progress.Add("currentWord", GetCurrentPidgenWord());
    }
}