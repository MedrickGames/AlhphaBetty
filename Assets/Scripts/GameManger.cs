using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class GameManger : MonoBehaviour
{
    private const string ApiUrl = "https://api.dictionaryapi.dev/api/v2/entries/en/";
    public string word = "";
    public string lastCheckedWord = "";
    private bool isChecking = false;
    public bool isAcceptable = false;
    public Tile lastTile = null;
    
    public List<Tile> selectedTiles = new List<Tile>();

    void Start()
    {
        // Example usage of CheckWord
        if (!string.IsNullOrEmpty(word) && word.Length >= 3)
        {
            StartCoroutine(CheckWord(word));
        }
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(word) && word != lastCheckedWord && !isChecking && word.Length >= 3)
        {
            lastCheckedWord = word.Trim();
            StartCoroutine(CheckWord(lastCheckedWord));
        }
    }

    public IEnumerator CheckWord(string wordToCheck)
    {
        isChecking = true;
        string fullUrl = ApiUrl + wordToCheck.ToLower();
        using (UnityWebRequest webRequest = UnityWebRequest.Get(fullUrl))
        {
            yield return webRequest.SendWebRequest();

            // Handle request result
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error checking word: " + webRequest.error);
                isAcceptable = false;
            }
            else if (webRequest.result == UnityWebRequest.Result.Success && webRequest.downloadHandler.text.Contains("word"))
            {
                Debug.Log("Word exists: " + wordToCheck);
                isAcceptable = true;
            }
            else
            {
                Debug.Log("Not a valid word: " + wordToCheck);
                isAcceptable = false;
            }
        }
        isChecking = false;
    }
}