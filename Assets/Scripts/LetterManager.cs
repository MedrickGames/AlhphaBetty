using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LetterManager : MonoBehaviour
{
    // Letter chance class within LetterManager
    [System.Serializable]
    public class LetterChance
    {
        public char letter;    // The letter (A-Z)
        public float chance;   // The chance for this letter
    }

    // List of letters and their respective chances
    public List<LetterChance> letterChances = new List<LetterChance>();

    // Automatically initialize chances for each letter from A-Z
    private void Awake()
    {
        if (letterChances.Count == 0) // Initialize only if list is empty
        {
            InitializeLetterChances();
        }
    }

    // This method initializes the list with word-based letter frequencies
    private void InitializeLetterChances()
    {
        // Letters ordered by frequency of use in words (high to low)
        string alphabet = "ETAOINSHRDLCUMWFGYPBVKJXQZ";

        // Example default chances based on word frequency
        float[] defaultChances = new float[]
        {
            12.70f, // E
            9.06f,  // T
            8.17f,  // A
            7.51f,  // O
            6.97f,  // I
            6.75f,  // N
            6.33f,  // S
            6.09f,  // H
            5.99f,  // R
            4.25f,  // D
            4.03f,  // L
            2.78f,  // C
            2.41f,  // U
            2.36f,  // M
            2.23f,  // W
            2.02f,  // F
            1.97f,  // G
            1.49f,  // Y
            0.98f,  // P
            0.77f,  // B
            0.15f,  // V
            0.10f,  // K
            0.15f,  // J
            0.09f,  // X
            0.07f,  // Q
            0.07f   // Z
        };

        // Populate the letterChances list with letters and their default chances
        for (int i = 0; i < alphabet.Length; i++)
        {
            LetterChance letterChance = new LetterChance
            {
                letter = alphabet[i],
                chance = defaultChances[i]
            };
            letterChances.Add(letterChance);
        }
    }

    // This method returns a randomly selected letter based on defined chances
    public char GetRandomLetter()
    {
        // Sum all the chances
        float totalChance = 0f;
        foreach (var letterChance in letterChances)
        {
            totalChance += letterChance.chance;
        }

        // Get a random value between 0 and totalChance
        float randomValue = Random.Range(0f, totalChance);

        // Find which letter corresponds to the random value
        float cumulativeChance = 0f;
        foreach (var letterChance in letterChances)
        {
            cumulativeChance += letterChance.chance;
            if (randomValue <= cumulativeChance)
            {
                return letterChance.letter;
            }
        }

        // Default case (shouldn't happen if probabilities are well-defined)
        return 'E'; // Default to the most common letter
    }
}
