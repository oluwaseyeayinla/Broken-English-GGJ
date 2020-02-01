using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LetterButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshProUGGUI;

    public string Letter
    {
        set { SetLetter(value); }
        get { return letter; }
    }



    //public Button.ButtonClickedEvent OnClick
    //{
    //    get { return cachedButton.onClick; }
    //}

    private string letter = string.Empty;
    private Button cachedButton;

    private void Awake()
    {
        cachedButton = GetComponent<Button>();
    }

    void SetLetter(string label)
    {
        
        letter = label;

        if (textMeshProUGGUI != null)
        {
            textMeshProUGGUI.text = label.ToUpper();
        }
    }

    public void OnClick()
    {
        LetterSlot letterSlot = transform.parent.GetComponent<LetterSlot>();
        
        if (letterSlot != null)
        {
            Debug.Log("Letter Slot => " + letterSlot.name);
            LetterRack letterRack = letterSlot.LetterRack;
            if (letterRack != null)
            {
                Debug.Log("Letter Rack => " + letterRack.name);
                letterRack.LetterButtonTapped(this);
            }
        }
    }
}
