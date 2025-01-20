using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using TMPro;

[System.Serializable]
public class PlayerStatsData
{
    public string playerName;
    public int gamesPlayed;
    public int gamesWon;
    public int gamesLost;
    public int score;
}

[System.Serializable]
public class StatsResponse
{
    public List<PlayerStatsData> stats;
}

public class StatsManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button backButton;
    [SerializeField] private Transform statsContainer;
    [SerializeField] private GameObject statRowPrefab;
    [SerializeField] private TMP_Text errorText;
    
    [Header("API Settings")]
    [SerializeField] private string apiUrl = "http://localhost:3367/api/players/stats";
    
    void Start()
    {
        backButton.onClick.AddListener(GoBack);
        StartCoroutine(FetchStats());
    }
    
    private IEnumerator FetchStats()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            Debug.Log($"Raw response: {request.downloadHandler.text}"); // Pour voir la réponse brute

            if (request.result != UnityWebRequest.Result.Success)
            {
                errorText.text = "Error loading stats: " + request.error;
                Debug.LogError($"Request failed: {request.error}");
            }
            else
            {
                try
                {
                    string jsonResponse = request.downloadHandler.text;
                    if (string.IsNullOrEmpty(jsonResponse))
                    {
                        errorText.text = "Received empty response from server";
                        yield break;
                    }

                    Debug.Log($"Attempting to parse JSON: {jsonResponse}");
                
                    if (!jsonResponse.Contains("\"stats\":"))
                    {
                        jsonResponse = "{\"stats\":" + jsonResponse + "}";
                    }

                    StatsResponse response = JsonUtility.FromJson<StatsResponse>(jsonResponse);
                
                    if (response == null || response.stats == null)
                    {
                        errorText.text = "Invalid response format";
                        Debug.LogError("Response or stats list is null");
                        yield break;
                    }

                    DisplayStats(response.stats);
                }
                catch (System.Exception e)
                {
                    errorText.text = "Error parsing stats data: " + e.Message;
                    Debug.LogError($"JSON Parse Error: {e}\nStack trace: {e.StackTrace}");
                }
            }
        }
    }
    
    private void DisplayStats(List<PlayerStatsData> stats)
    {
        // Clear existing stats
        foreach (Transform child in statsContainer)
        {
            Destroy(child.gameObject);
        }

        // Create new rows for each player
        foreach (var playerStats in stats)
        {
            GameObject row = Instantiate(statRowPrefab, statsContainer);
            var texts = row.GetComponentsInChildren<TMP_Text>();
            
            // Assuming the texts are in order: Username, Games, Wins, Losses, Score
            texts[0].text = playerStats.playerName;
            texts[1].text = playerStats.gamesPlayed.ToString();
            // texts[2].text = playerStats.gamesWon.ToString();
            // texts[3].text = playerStats.gamesLost.ToString();
            texts[2].text = playerStats.score.ToString();
        }
    }

    public void GoBack()
    {
        // Load the Home Scene
        SceneManager.LoadScene("HomeScene");
    }
}
