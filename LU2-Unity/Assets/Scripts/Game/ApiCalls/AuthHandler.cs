using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private GameObject loginRegisterPanel, loadingPanel;
    [SerializeField] private Button loginButton, registerButton, submitButton, backButton;
    [SerializeField] private TMP_InputField usernameField, passwordField;
    private bool isLogin = true;

    private void Start()
    {
        string savedToken = PlayerPrefs.GetString("authToken", "");
        if (!string.IsNullOrEmpty(savedToken))
        {
            APIManager.Instance.SetAuthToken(savedToken);
            APIManager.Instance.GetRequest("/account/username", OnUsernameReceived);
        }
        else
        {
            ResetUI();
        }
    }

    private void OnUsernameReceived(APIResponse response)
    {
        loadingPanel.SetActive(true);
        if (response.Success && SceneManager.GetActiveScene().buildIndex == 0)
        {
            SceneManager.LoadScene(1);
        }
        ResetUI();
        loadingPanel.SetActive(false);
    }

    private void ShowInput(bool show)
    {
        loginButton.gameObject.SetActive(!show);
        registerButton.gameObject.SetActive(!show);

        usernameField.gameObject.SetActive(show);
        passwordField.gameObject.SetActive(show);
        submitButton.gameObject.SetActive(show);
        backButton.gameObject.SetActive(show);

        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(SubmitRequest);

        resultText.gameObject.SetActive(true);
    }

    public void OnButtonPressed(string state)
    {
        switch (state)
        {
            case "login":
                isLogin = true;
                ShowInput(true);
                break;
            case "register":
                isLogin = false;
                ShowInput(true);
                break;
            case "back":
                resultText.text = "";
                ShowInput(false);
                break;
        }
    }

    private void SubmitRequest()
    {
        string jsonData = $"{{\"userName\":\"{usernameField.text}\",\"password\":\"{passwordField.text}\"}}";
        string endpoint = isLogin ? "/account/login" : "/account/register";
        APIManager.Instance.PostRequest(endpoint, jsonData, OnResponse);
    }

    private void OnResponse(APIResponse response)
    {
        if (response.Success)
        {
            resultText.text = isLogin ? "Ingelogd" : "Account aangemaakt";

            if (isLogin)
            {
                var jsonResponse = JsonUtility.FromJson<AuthResponse>(response.Data);

                if (jsonResponse != null && !string.IsNullOrEmpty(jsonResponse.token))
                {
                    PlayerPrefs.SetString("authToken", jsonResponse.token);
                    PlayerPrefs.Save();
                    APIManager.Instance.SetAuthToken(jsonResponse.token);
                    SceneManager.LoadScene(1);
                }
            }

            ResetUI();
        }
        else
        {
            resultText.text = response.StatusCode switch
            {
                400 => "Ongeldige gebruikersnaam of wachtwoord.",
                503 => "Serverfout, probeer het later opnieuw.",
                _ => "Er is een fout opgetreden."
            };
        }
    }
    private class AuthResponse
    {
        public string token;
    }
    private void ResetUI()
    {
        ShowInput(false);
        usernameField.text = "";
        passwordField.text = "";
        resultText.gameObject.SetActive(true);
    }

    public void Logout()
    {
        PlayerPrefs.DeleteKey("authToken");
        APIManager.Instance.SetAuthToken("");
        SceneManager.LoadScene(0);
    }
}
