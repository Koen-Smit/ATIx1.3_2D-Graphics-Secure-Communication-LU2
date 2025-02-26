using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Assets.Scripts.Api.Models;
using System;
using Unity.VisualScripting.Antlr3.Runtime;

public class ApiClient : MonoBehaviour
{
    private string apiUrl = "https://avansict2227609.azurewebsites.net";
    private string loggedInUsername;
    private string authToken;

    void Start()
    {
        Account testAccount = new Account { userName = "Koen", password = "Test123456!" };
        StartCoroutine(Login(testAccount));
        GetUsername();
        Logout();
    }

    // LOGIN REQUEST
    public IEnumerator Login(Account account)
    {

        string jsonData = JsonUtility.ToJson(account);
        using (UnityWebRequest request = new UnityWebRequest(apiUrl + "/account/login", "POST"))
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            string rawResponse = request.downloadHandler.text;

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(rawResponse);
                    if (loginResponse != null)
                    {
                        PlayerPrefs.SetString("Token", authToken);
                        authToken = PlayerPrefs.GetString("Token", "");
                        Debug.Log("Auth Token received: " + authToken);

                    }
                    else
                    {
                        Debug.LogError("Login successful, but no token received!");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("JSON Parsing Error: " + e.Message);
                }
            }
            else
            {
                Debug.LogError("Login failed! Error: " + request.error);
            }
        }
    }




    // LOGOUT REQUEST
    public IEnumerator Logout()
    {
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(apiUrl + "/account/logout", ""))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            HandleResponse(request, "Logout");
        }
    }

    // GET USERNAME REQUEST
    public IEnumerator GetUsername()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl + "/account/username"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                loggedInUsername = request.downloadHandler.text;
                Debug.Log("Username: " + loggedInUsername);
            }
            else
            {
                Debug.LogError("Error getting username: " + request.error);
            }
        }
    }




    // Handle API Response
    private void HandleResponse(UnityWebRequest request, string requestType)
    {
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(requestType + " successful! Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError(requestType + " failed! Error: " + request.error);
        }
    }
}
