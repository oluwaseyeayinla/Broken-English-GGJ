using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] Score _currentScore;

    [Header("Timer")]
    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] Score _currentTimer;

    [Header("Progress")]
    [SerializeField] Image _progressBar;
    [SerializeField] Tally _currentRepairRatio;
}
