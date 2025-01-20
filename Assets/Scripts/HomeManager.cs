using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class HomeManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button statsButton;
    [SerializeField] private Button feedbackButton;

    [Header("Settings")]
    [SerializeField] private string StatsSceneName = "StatsScene";
    [SerializeField] private string FeedbackSceneName = "FeedbackScene";
    
    [SerializeField] private TMP_Text welcomeText;
    void Start()
    {
        string username = PlayerPrefs.GetString("Username", "User");
        welcomeText.text = $"{username} !";
        
        statsButton.onClick.AddListener(GoToStats);
        feedbackButton.onClick.AddListener(GoToFeedback);
    }
    
    public void GoToStats()
    {
        SceneManager.LoadScene(StatsSceneName);
    }
    
    public void GoToFeedback()
    {
        SceneManager.LoadScene(FeedbackSceneName);
    }
}