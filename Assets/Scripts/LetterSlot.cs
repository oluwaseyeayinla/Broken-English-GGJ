using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterSlot : MonoBehaviour
{
    [SerializeField] bool _isOccupied = false;

    private LetterButton letterButton = null;

    public bool IsOccupied
    {
        get { return _isOccupied; }
    }

    public LetterButton LetterButton
    {
        get { return letterButton; }
    }

    public LetterRack LetterRack
    {
		get { return this.transform.parent.gameObject.GetComponent<LetterRack>(); }
    }

    public void AddToSlot(LetterButton letterButton)
    {
        this.letterButton = letterButton;
		letterButton.transform.SetParent(this.transform, false);
        _isOccupied = true;
    }

    public void ClearSlot()
    {
        letterButton = null;
        _isOccupied = false;
    }

}

