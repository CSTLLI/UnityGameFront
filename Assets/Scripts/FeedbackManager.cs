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

[System.Serializable]
public class LastFeedbackResponse
{
    public bool success;
    public FeedbackDetails feedback;
}

[System.Serializable]
public class FeedbackDetails
{
    public int id;
    public string comment;
}

public class FeedbackManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField feedbackInput;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text lastFeedBackText;
    [SerializeField] private TMP_Text lastFeedBackLabelText;

    private int currentFeedbackId = -1;

    [Header("API Settings")]
    [SerializeField] private string apiUrl = "http://localhost:3367/api/feedback";
    [SerializeField] private string getLastFeedbackUrl = "http://localhost:3367/api/feedback/last/";

    void Start()
    {
        // Vérifier que toutes les références sont assignées
        if (!feedbackInput || !submitButton || !statusText || !lastFeedBackText || !lastFeedBackLabelText)
        {
            Debug.LogError("Missing UI references in FeedbackManager!");
            return;
        }

        // Initialiser les listeners
        submitButton.onClick.AddListener(SubmitFeedback);
        backButton.onClick.AddListener(GoBack);
        
        // Effacer le message de statut
        statusText.text = "";
        
        // Récupérer le dernier feedback
        StartCoroutine(GetLastFeedback());
    }

private IEnumerator GetLastFeedback()
{
    string userId = PlayerPrefs.GetString("UserID");
    string url = getLastFeedbackUrl + userId;

    Debug.Log(url);
    
    using (UnityWebRequest request = UnityWebRequest.Get(url))
    {
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            LastFeedbackResponse response = JsonUtility.FromJson<LastFeedbackResponse>(request.downloadHandler.text);

            Debug.Log("Response success: " + response.success);
            Debug.Log("Response feedback: " + (response.feedback != null ? "not null" : "null"));

            if (response.success)
            {
                if (request.downloadHandler.text.Contains("\"feedback\":null"))
                {
                    currentFeedbackId = -1;
                    lastFeedBackLabelText.text = "Donnez nous votre avis!";
                    lastFeedBackText.text = "";
                    submitButton.GetComponentInChildren<TMP_Text>().text = "Envoyer";
                    feedbackInput.text = "";
                }
                else
                {
                    currentFeedbackId = response.feedback.id;
                    lastFeedBackLabelText.text = "Votre dernier avis :";
                    lastFeedBackText.text = response.feedback.comment;
                    submitButton.GetComponentInChildren<TMP_Text>().text = "Mettre à jour";
                    feedbackInput.text = "";
                }
            }
            else
            {
                Debug.LogError("API response indicates failure");
                lastFeedBackLabelText.text = "Erreur lors de la récupération";
                lastFeedBackText.text = "";
            }
        }
        else
        {
            Debug.LogError($"Error getting last feedback: {request.error}");
            lastFeedBackLabelText.text = "Erreur lors de la récupération";
            lastFeedBackText.text = "";
        }
    }
}

    private IEnumerator SendFeedback()
    {
        submitButton.interactable = false;
        statusText.text = "Envoi en cours...";

        var feedbackData = new FeedbackData
        {
            playerId = PlayerPrefs.GetString("UserID"),
            comment = feedbackInput.text
        };

        string jsonData = JsonUtility.ToJson(feedbackData);
        string methodUrl = currentFeedbackId > 0 ? $"{apiUrl}/{currentFeedbackId}" : apiUrl;
        string method = currentFeedbackId > 0 ? "PUT" : "POST";

        using (var request = new UnityWebRequest(methodUrl, method))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                statusText.text = "Erreur lors de l'envoi : " + request.error;
                Debug.LogError($"Feedback submission failed: {request.error}");
            }
            else
            {
                statusText.text = currentFeedbackId > 0 ? "Avis mis à jour !" : "Avis envoyé !";
                yield return new WaitForSeconds(2);
                GoBack();
            }
        }

        submitButton.interactable = true;
    }

    private void SubmitFeedback()
    {
        if (string.IsNullOrEmpty(feedbackInput.text))
        {
            statusText.text = "Veuillez entrer votre avis";
            return;
        }

        StartCoroutine(SendFeedback());
    }

    private void GoBack()
    {
        SceneManager.LoadScene("HomeScene");
    }
}