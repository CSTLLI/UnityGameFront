using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using TMPro;

public class LoginManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_Text errorText;

    [Header("Settings")]
    [SerializeField] private string apiUrl = "http://localhost:3367/api/auth/login";
    [SerializeField] private string homeSceneName = "HomeScene";
    
    [System.Serializable]
    private class LoginRequest
    {
        public string username;
        public string password;
    }

    private void Start()
    {
        if (!usernameInput || !passwordInput || !loginButton || !errorText)
        {
            Debug.LogError("Missing UI references in LoginManager!");
            return;
        }

        errorText.text = "";
        loginButton.onClick.AddListener(HandleLogin);
        passwordInput.onSubmit.AddListener((_) => HandleLogin());
    }

    private void HandleLogin()
    {
        if (string.IsNullOrEmpty(usernameInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            errorText.text = "Please enter both username and password";
            return;
        }

        StartCoroutine(LoginCoroutine());
    }

    private IEnumerator LoginCoroutine()
    {
        loginButton.interactable = false;
        errorText.text = "Logging in...";

        var loginRequest = new LoginRequest
        {
            username = usernameInput.text,
            password = passwordInput.text
        };

        string jsonData = JsonUtility.ToJson(loginRequest);
        Debug.Log($"Sending data: {jsonData}");

        var request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send request
        yield return request.SendWebRequest();
        
        Debug.Log($"Response: {request.downloadHandler.text}");
        Debug.Log($"Response Code: {request.responseCode}");

        // Check for errors
        if (request.result == UnityWebRequest.Result.ConnectionError || 
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            errorText.text = "Login failed: " + request.error;
            loginButton.interactable = true;
        }
        else
        {
            // Parse response
            LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

            if (response.success)
            {
                // Store user data if needed
                PlayerPrefs.SetString("UserID", response.user.id.ToString());
                PlayerPrefs.SetString("Username", response.user.username);
                PlayerPrefs.Save();

                // Load home scene
                SceneManager.LoadScene(homeSceneName);
            }
            else
            {
                errorText.text = "Invalid username or password";
                loginButton.interactable = true;
            }
        }

        request.Dispose();
    }
}

[System.Serializable]
public class LoginResponse
{
    public bool success;
    public UserData user;
}

[System.Serializable]
public class UserData
{
    public int id;
    public string username;
}