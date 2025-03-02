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
        APIManager.Instance.GetRequest("/account/username", OnUsernameReceived);
    }
    private void OnUsernameReceived(APIResponse response)
    {
        loadingPanel.SetActive(true);
        if (response.Success && SceneManager.GetActiveScene().buildIndex == 0)
            SceneManager.LoadScene(1);
        ResetUI();
        loadingPanel.SetActive(false);
    }

    private void ShowInput(bool show)
    {
        if (show)
        {
            loginButton.gameObject.SetActive(false);
            registerButton.gameObject.SetActive(false);

            usernameField.gameObject.SetActive(true);
            passwordField.gameObject.SetActive(true);
            submitButton.gameObject.SetActive(true);
            backButton.gameObject.SetActive(true);

            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(SubmitRequest);
        }
        else
        {
            loginButton.gameObject.SetActive(true);
            registerButton.gameObject.SetActive(true);

            usernameField.gameObject.SetActive(false);
            passwordField.gameObject.SetActive(false);
            submitButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);
        }

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
            resultText.gameObject.SetActive(true);
            resultText.text = isLogin ? "Ingelogd" : "Account aangemaakt";
            if (isLogin)
                SceneManager.LoadScene(1);

            resultText.gameObject.SetActive(true);
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
    private void ResetUI()
    {
        ShowInput(false);
        usernameField.text = "";
        passwordField.text = "";
        resultText.gameObject.SetActive(true);
    }
}
