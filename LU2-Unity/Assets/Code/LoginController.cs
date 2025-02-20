using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public Button registerButton;
    public Button loginButton;
    public TextMeshProUGUI feedbackText;

    [Header("Dependencies")]
    public ExampleApp exampleApp;

    private void Start()
    {
        // Assign button click listeners
        registerButton.onClick.AddListener(RegisterUser);
        loginButton.onClick.AddListener(LoginUser);
    }

    private void RegisterUser()
    {
        // Set user data in ExampleApp before making API call
        exampleApp.user.email = emailField.text;
        exampleApp.user.password = "33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!";

        // Call register function
        exampleApp.Register();

        // Provide feedback
        feedbackText.text = "Registering...";
    }

    private void LoginUser()
    {
        // Set user data in ExampleApp before making API call
        exampleApp.user.email = emailField.text;
        exampleApp.user.password = "33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!33nW@chtWoord!";

        // Call login function
        exampleApp.Login();

        // Provide feedback
        feedbackText.text = "Logging in...";
    }

    // Function to be called from ExampleApp when login/register is successful
    public void ShowSuccessMessage(string message)
    {
        feedbackText.text = message;
    }
}
