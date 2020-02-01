using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterRack : MonoBehaviour
{
    [SerializeField] GameObject _slotPrefab;
    [SerializeField] LetterRack _dependentRack;

    private List<LetterSlot> slots = new List<LetterSlot>();

    public void SpawnSlots(int amount)
    {
        slots = new List<LetterSlot>();
        for (int i = 0; i < amount; i++)
        {
            GameObject clone = Instantiate(_slotPrefab, transform);
            clone.SetActive(true);
            slots.Add(clone.GetComponent<LetterSlot>());
        }
    }

    private LetterSlot GetFirstEmptySlot()
    {
        foreach (LetterSlot slot in slots)
        {
            if (slot != null && !slot.IsOccupied)
            {
                return slot;
            }
        }
        // No empty slots
        return null;
    }

    //Add a tile to the first empty slot
    public void AddLetterButtonToFirstEmptySlot(LetterButton letterButton)
    {
        LetterSlot emptySlot = GetFirstEmptySlot();
        if (emptySlot != null)
        {
            emptySlot.AddToSlot(letterButton);
        }
    }

    //Add a tile to a specific slot
    //Checks if slot is in rack, that slot is empty then adds tile
    public void AddTileToSlot(LetterButton letterButton, LetterSlot letterSlot)
    {
        if (ContainsSlot(letterSlot))
        {
            if (!letterSlot.IsOccupied)
            {
                letterSlot.AddToSlot(letterButton);
            }
        }
    }

    //Looping over array to find element.  V.inefficient.  IndexOf works with Android?
    //Remove tile from a specific slot
    public void RemoveTileFromSlot(LetterButton letterButton, LetterSlot letterSlot)
    {
        if (ContainsLetterButton(letterButton) && ContainsSlot(letterSlot))
        {
            if (letterSlot.LetterButton == letterButton)
            {
                letterSlot.ClearSlot();
            }
        }
    }

    public void RemoveLetterButton(LetterButton letterButton)
    {
        if (ContainsLetterButton(letterButton))
        {
            LetterSlot slot = GetSlotContainingLetterButton(letterButton);
            slot.ClearSlot();
        }
    }

    //Loop over all slots and to check for tile
    public bool ContainsLetterButton(LetterButton letterButtonToFind)
    {
        foreach (LetterSlot slot in slots)
        {
            if (slot.LetterButton == letterButtonToFind)
            {
                return true;
            }
        }
        return false;
    }

    //Check if slot is in this rack
    private bool ContainsSlot(LetterSlot slotToFind)
    {
        foreach (LetterSlot slot in slots)
        {
            if (slot == slotToFind)
            {
                return true;
            }
        }
        return false;
    }

    public LetterSlot GetSlotContainingLetterButton(LetterButton letterButton)
    {
        foreach (LetterSlot slot in slots)
        {
            if (slot.LetterButton == letterButton)
            {
                return slot;
            }
        }
        return null;
    }

    //Returns null if there is no tile in the slot
    public LetterButton GetLetterButtonInSlot(LetterSlot slot)
    {
        return slot.LetterButton;
    }

    //Returns all the tiles in the current rack
    public LetterButton[] GetTiles()
    {
        List<LetterButton> letters = new List<LetterButton>();
        foreach (LetterSlot slot in slots)
        {
            LetterButton letterButton = slot.LetterButton;
            if (letterButton != null)
            {
                letters.Add(letterButton);
            }
        }

        return letters.ToArray();
    }

    //TODO:
    //Compare shuffled list to starting list
    //Keep shuffling until they are different
    //handle case of 1 rack shuffle
    public void Shuffle()
    {
        List<LetterButton> lettersToShuffle = new List<LetterButton>();
        //Remove tiles from slots and add to a list
        foreach (LetterSlot slot in slots)
        {
            if (slot.IsOccupied)
            {
                lettersToShuffle.Add(slot.LetterButton);
                slot.ClearSlot();
            }
        }
        //Shuffle the list
        LetterButton[] letters = lettersToShuffle.ToArray();
        for (int i = 0; i < letters.Length; i++)
        {
            LetterButton temp = letters[i];
            int randomIndex = Random.Range(i, letters.Length);
            letters[i] = letters[randomIndex];
            letters[randomIndex] = temp;
        }
        //Add back to the slots

        int tilesToAddIndex = 0;

        //Only add tiles slots that are empty in the play rack 
        for (int i = 0; i < slots.Count; i++)
        {
            Debug.Log(_dependentRack.slots[i].IsOccupied);
            if (_dependentRack.slots[i].IsOccupied == false)
            {
                AddTileToSlot(letters[tilesToAddIndex], slots[i]);
                tilesToAddIndex++;
            }
        }
    }

    //Loop over slots.  For those that have a tile, add its letter to the string
    public string GetRackString()
    {
        string rackString = "";
        foreach (LetterSlot slot in slots)
        {
            if (slot.IsOccupied)
            {
                rackString += slot.LetterButton.Letter;
            }
        }
        return rackString;
    }

    //Delete all the tiles and clear all the slots
    public void ClearRack()
    {
        foreach (LetterSlot slot in slots)
        {
            LetterButton letterButton = GetLetterButtonInSlot(slot);
            if (letterButton != null)
            {
                Destroy(letterButton.gameObject);
                slot.ClearSlot();
            }
        }
    }

    public int GetNumOccupiedSlots()
    {
        int occupiedSlots = 0;
        foreach (LetterSlot slot in slots)
        {
            if (slot.IsOccupied)
            {
                occupiedSlots++;
            }
        }
        return occupiedSlots;
    }

    public int GetNumUnoccupiedSlots()
    {
        int unoccupiedSlots = 0;
        foreach (LetterSlot slot in slots)
        {
            if (slot.IsOccupied == false)
            {
                unoccupiedSlots++;
            }
        }
        return unoccupiedSlots;
    }

    //Recall all tiles from other rack to this rack
    public void RecallTilesToRack()
    {
        LetterButton[] letters = _dependentRack.GetTiles();
        foreach (LetterButton letterButton in letters)
        {
            _dependentRack.RemoveLetterButton(letterButton);
            AddLetterButtonToFirstEmptySlot(letterButton);
        }
    }

    public void LetterButtonTapped(LetterButton letterButton)
    {
        Debug.Log("LetterButtonTapped: " + letterButton.name);
        RemoveLetterButton(letterButton);
        _dependentRack.AddLetterButtonToFirstEmptySlot(letterButton);
    }
}
