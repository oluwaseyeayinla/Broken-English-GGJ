using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static string GetRandomLetter()
    {
        string[] alphabet =
            {
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
                "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
                "U", "V", "W", "X", "Y", "Z"
            };

        return alphabet[Random.Range(0, alphabet.Length)];
    }
}
