using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using System.Text.RegularExpressions;

public class AuthManager : MonoBehaviour
{
    private string apiUrl = "https://avansict2227609.azurewebsites.net";
    private string authToken;

    [SerializeField] private TMP_Text statusText;
    [SerializeField] private GameObject loginRegisterPanel;
    [SerializeField] private Button loginButton, registerButton, submitButton, logoutButton, backButton;
    [SerializeField] private TMP_InputField usernameField, passwordField;

    private bool isRegistering = false;
    private void Start()
    {
        authToken = "";
        authToken = PlayerPrefs.GetString("Token", "");
        if (!string.IsNullOrEmpty(authToken))
        {
            ShowLoggedInScreen();
        }
        else
        {
            ShowLoginRegisterOptions();
        }
    }

    private void ShowLoggedInScreen()
    {
        loginRegisterPanel.SetActive(false);
        statusText.text = "";
        logoutButton.gameObject.SetActive(true);
        StartCoroutine(GetUsername());
    }

    private void ShowLoginRegisterOptions()
    {
        loginRegisterPanel.SetActive(true);
        loginButton.gameObject.SetActive(true);
        registerButton.gameObject.SetActive(true);
        usernameField.gameObject.SetActive(false);
        passwordField.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        logoutButton.gameObject.SetActive(false);
    }

    public void OnLoginButtonPressed()
    {
        isRegistering = false;
        ShowInputFields();
    }

    public void OnRegisterButtonPressed()
    {
        isRegistering = true;
        ShowInputFields();
    }

    public void OnBackButtonPressed()
    {
        ShowLoginRegisterOptions();
    }

    private IEnumerator GetUsername()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl + "/account/username"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                statusText.text = "" + request.downloadHandler.text;
            }
            else
            {
                statusText.text = "Error getting username: " + request.error;
            }
        }
    }


    private void ShowInputFields()
    {
        statusText.text = "";
        loginButton.gameObject.SetActive(false);
        registerButton.gameObject.SetActive(false);
        usernameField.gameObject.SetActive(true);
        passwordField.gameObject.SetActive(true);
        submitButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(() => StartCoroutine(isRegistering ? Register() : Login()));
    }


    private IEnumerator Register()
    {
        statusText.text = "";
        string username = usernameField.text.Trim();
        string password = passwordField.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            statusText.text = "Please enter a username and password.";
            yield break;
        }

        if (!IsValidPassword(password))
        {
            statusText.text = "Password not valid.";
            yield break;
        }

        Account registerData = new Account { UserName = username, Password = password };
        string jsonData = JsonUtility.ToJson(registerData);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl + "/account/register", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                statusText.text = "";
                ShowLoginRegisterOptions();
            }
            else
            {
                if (request.downloadHandler.text.Contains("already exists"))
                {
                    statusText.text = "Username is already taken.";
                }
                else
                {
                    statusText.text = "Registration failed: " + request.downloadHandler.text;
                }
            }
        }
    }

    private bool IsValidPassword(string password)
    {
        return password.Length >= 10 &&
               Regex.IsMatch(password, @"[A-Z]") &&
               Regex.IsMatch(password, @"[a-z]") &&
               Regex.IsMatch(password, @"\d") && 
               Regex.IsMatch(password, @"\W");
    }


private IEnumerator Login()
    {
        statusText.text = "";
        string username = usernameField.text.Trim();
        string password = passwordField.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            statusText.text = "Please enter a username and password.";
            yield break;
        }

        Account loginData = new Account { UserName = username, Password = password };
        string jsonData = JsonUtility.ToJson(loginData);


        using (UnityWebRequest request = new UnityWebRequest(apiUrl + "/account/login", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
                authToken = response.token;
                PlayerPrefs.SetString("Token", authToken);
                PlayerPrefs.Save();
                ShowLoggedInScreen();
            }
            else
            {
                statusText.text = "Login failed: " + request.downloadHandler.text;
            }
        }
    }




    public void OnLogoutButtonPressed()
    {
        StartCoroutine(Logout());
        authToken = "";
    }


    private IEnumerator Logout()
    {
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(apiUrl + "/account/logout", ""))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                PlayerPrefs.DeleteKey("Token");
                PlayerPrefs.Save();
                ShowLoginRegisterOptions();
                statusText.text = "Logged out";
            }
            else
            {
                statusText.text = "Logout failed: " + request.error;
            }
        }
    }
}
