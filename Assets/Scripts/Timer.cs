using System;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] float _startValueInSeconds;
    [SerializeField] float _currentValueInSeconds;

    public float CurrentValueInSeconds
    {
        set { _currentValueInSeconds = value; }
        get { return _currentValueInSeconds; }
    }

    public event Action OnTimerCompleted;

    bool isRunning = false;
    bool isTimeOut = false;

    public void Reset()
    {
        CurrentValueInSeconds = _startValueInSeconds;
        isRunning = false;
        isTimeOut = false;
    }

    public void Begin()
    {
        isRunning = true;
    }

    // Update is called once per frame
    public void Update()
    {
        if (isRunning)
        {
            if (Time.timeSinceLevelLoad >= 1 && !isTimeOut)
            {
                _currentValueInSeconds -= Time.deltaTime;
                if (FloatEquals(_currentValueInSeconds, 0))
                {
                    isTimeOut = true;
                    if (OnTimerCompleted != null)
                    {
                        OnTimerCompleted.Invoke();
                    }
                }
            }
        }
    }

    public void End()
    {
        isRunning = false;
        isTimeOut = false;
    }

    bool FloatEquals(float num1, float num2, float threshold = .0001f)
    {
        return Math.Abs(num1 - num2) < threshold;
    }
}
