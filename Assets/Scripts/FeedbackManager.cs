using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

[System.Serializable]
public class FeedbackData
{
    public string playerId;
    public string comment;
}

public class FeedbackManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField feedbackInput;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_Text statusText;

    [Header("API Settings")]
    [SerializeField] private string apiUrl = "http://localhost:3367/api/feedback";

    void Start()
    {
        // Vérifier que toutes les références sont assignées
        if (!feedbackInput || !submitButton || !statusText)
        {
            Debug.LogError("Missing UI references in FeedbackManager!");
            return;
        }

        // Initialiser les listeners
        submitButton.onClick.AddListener(SubmitFeedback);
        backButton.onClick.AddListener(GoBack);
        
        // Effacer le message de statut
        statusText.text = "";
    }

    private void SubmitFeedback()
    {
        // Vérifier que le feedback n'est pas vide
        if (string.IsNullOrEmpty(feedbackInput.text))
        {
            statusText.text = "Please enter your feedback";
            return;
        }

        StartCoroutine(SendFeedback());
    }

    private IEnumerator SendFeedback()
    {
        // Désactiver le bouton pendant l'envoi
        submitButton.interactable = false;
        statusText.text = "Sending feedback...";

        // Créer l'objet de données
        var feedbackData = new FeedbackData
        {
            playerId = PlayerPrefs.GetString("UserID"),
            comment = feedbackInput.text
        };

        // Convertir en JSON
        string jsonData = JsonUtility.ToJson(feedbackData);

        // Créer la requête
        using (var request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                statusText.text = "Error sending feedback: " + request.error;
                Debug.LogError($"Feedback submission failed: {request.error}");
            }
            else
            {
                statusText.text = "Feedback sent successfully!";
                feedbackInput.text = "";
                
                yield return new WaitForSeconds(2);
                GoBack();
            }
        }

        submitButton.interactable = true;
    }

    private void GoBack()
    {
        SceneManager.LoadScene("HomeScene");
    }
}