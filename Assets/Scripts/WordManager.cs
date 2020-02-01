using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    public TextAsset rawTextFile;
    public int maxWords;
    public bool randomise;
    public string currentWord;
    public string shuffledWord;
    public bool debugMode;


    public Hashtable unsolvedWords;
    public Hashtable solvedWords;
    

    public void Setup()
    {
        LoadWordsFromTextFile();
        SetCurrentWord(GetUnsolvedWord());
        solvedWords = new Hashtable();
    }

    public void Setup(Hashtable data)
    {
        solvedWords = (Hashtable)data["solvedWords"];
        unsolvedWords = (Hashtable)data["unsolvedWords"];
        currentWord = (string)data["currentWord"];
    }

    public int GetNumSolvedWords()
    {
        if (solvedWords != null)
        {
            return solvedWords.Count;
        }

        return 0;
    }

    public int GetNumUnsolvedWords()
    {
        if (unsolvedWords != null)
        {
            return unsolvedWords.Count;
        }

        return 0;
    }

    public string GetUnsolvedWord()
    {
        ICollection keys = unsolvedWords.Keys;

        string[] keyArray = new string[keys.Count];
        keys.CopyTo(keyArray, 0);
        int index = randomise ? Random.Range(0, keys.Count) : 0;
        return (string)keyArray[index];
    }

    //Move a word from one hashtable to the other
    public void MarkWordAsSolved(string word)
    {
        unsolvedWords.Remove(word);
        solvedWords.Add(word, word);
    }

    public string GetCurrentWord()
    {
        return !debugMode ? currentWord : CreateRandomWord(5);
    }

    public void SetCurrentWord(string newCurrentWord)
    {
        currentWord = newCurrentWord;
    }

    public string ShuffleWord(string word)
    {
        shuffledWord = word;

        while (word != null && shuffledWord == word)
        {
            char[] chars = word.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                char temp = chars[i];
                int randomIndex = Random.Range(i, chars.Length);
                chars[i] = chars[randomIndex];
                chars[randomIndex] = temp;
            }

            shuffledWord = new string(chars);
        }

        return shuffledWord;
    }

    public void LoadWordsFromTextFile()
    {
        //Create new hashtable
        unsolvedWords = new Hashtable();
        //should handle duplicates

        string rawWords = rawTextFile.text;
        string[] rawLines = rawWords.Split('\n');
        foreach (string line in rawLines)
        {
			Debug.Log(line);
            // Pidgin -> Left Side - Index 0 | Repair => Right Side - Indexx 1
            string[] words = line.Split(',');
            unsolvedWords.Add(words[0].ToUpper(), words[1].ToUpper());
        }
    }

    public string CreateRandomWord(int count)
    {
        string result = string.Empty;

        for (int i = 0; i < count; i++)
        {
            result += Utility.GetRandomLetter();
        }

        return result;
    }
}