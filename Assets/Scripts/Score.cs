using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] int _defaultValue;
    [SerializeField] int _currentValue;

    public int CurrentValue
    {
        set
        {
            _currentValue = value;
            if (OnValueChanged != null)
            {
                OnValueChanged.Invoke(_currentValue);
            }
        }
        get { return _currentValue; }
    }

    public void Reset()
    {
        CurrentValue = _defaultValue;
    }

    public event Action<int> OnValueChanged;
}
