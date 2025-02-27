//using System.Collections;
//using UnityEngine;
//using UnityEngine.Networking;
//using System.Text;
//using System;

//public class ApiClient : MonoBehaviour
//{
//    private string apiUrl = "https://avansict2227609.azurewebsites.net";
//    private string loggedInUsername;
//    private string authToken;
//    private float tokenExpiryTime;
//    void Start()
//    {
//        //StartCoroutine(Logout());
//    }

//    // LOGIN REQUEST
//    public IEnumerator Login(Account account)
//    {
//        string jsonData = JsonUtility.ToJson(account);
//        using (UnityWebRequest request = new UnityWebRequest(apiUrl + "/account/login", "POST"))
//        {
//            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);
//            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
//            request.downloadHandler = new DownloadHandlerBuffer();
//            request.SetRequestHeader("Content-Type", "application/json");

//            yield return request.SendWebRequest();

//            string rawResponse = request.downloadHandler.text;

//            if (request.result == UnityWebRequest.Result.Success)
//            {
//                //Debug.Log("Login successful. Response: " + rawResponse);

//                try
//                {
//                    LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(rawResponse);
//                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.token))
//                    {
//                        authToken = loginResponse.token;
//                        PlayerPrefs.SetString("Token", authToken);
//                        tokenExpiryTime = Time.time + 1800;
//                        //Debug.Log("Login Successful! Token will expire at: " + tokenExpiryTime);
//                        StartCoroutine(RefreshTokenCoroutine());
//                    }
//                    else
//                    {
//                        //Debug.LogError("Login successful, but no token received! Response: " + rawResponse);
//                    }
//                }
//                catch (Exception)
//                {
//                    //Debug.LogError("JSON Parsing Error: " + e.Message);
//                }
//            }
//            else
//            {
//                //Debug.LogError("Login failed! Error: " + request.error + "\nResponse: " + rawResponse);
//            }
//        }
//    }

//    // LOGOUT REQUEST
//    public IEnumerator Logout()
//    {
//        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(apiUrl + "/account/logout", ""))
//        {
//            request.downloadHandler = new DownloadHandlerBuffer();
//            yield return request.SendWebRequest();
//            HandleResponse(request, "Logout");
//            if (request.result == UnityWebRequest.Result.Success)
//            {
//                authToken = "";
//                tokenExpiryTime = 0;
//                PlayerPrefs.DeleteAll();
//                PlayerPrefs.Save();
//                ClearUsername();
//            }
//            else
//            {
//                //Debug.LogError("Logout failed! Error: " + request.error);
//            }
//        }
//    }

//    // GET USERNAME REQUEST
//    public IEnumerator GetUsername()
//    {
//        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl + "/account/username"))
//        {
//            yield return request.SendWebRequest();

//            if (request.result == UnityWebRequest.Result.Success)
//            {
//                loggedInUsername = request.downloadHandler.text;
//                Debug.Log("Username: " + loggedInUsername);
//            }
//            else
//            {
//                //Debug.LogError("Error getting username: " + request.error);
//            }
//        }
//    }

//    // Handle API Response
//    private void HandleResponse(UnityWebRequest request, string requestType)
//    {
//        if (request.result == UnityWebRequest.Result.Success)
//        {
//            //Debug.Log(requestType + " successful! Response: " + request.downloadHandler.text);
//        }
//        else
//        {
//            //Debug.LogError(requestType + " failed! Error: " + request.error);
//        }
//    }

//    // Coroutine to refresh the token
//    private IEnumerator RefreshTokenCoroutine()
//    {
//        while (true)
//        {
//            // Check if token is near expiry
//            if (Time.time >= tokenExpiryTime - 60)
//            {
//                //Debug.Log("Token is about to expire. Refreshing...");
//                yield return StartCoroutine(RenewToken());
//            }

//            // 30 seconds before checking again
//            yield return new WaitForSeconds(30);
//        }
//    }

//    // Method to renew the token
//    private IEnumerator RenewToken()
//    {
//        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl + "/account/renew", new WWWForm()))
//        {
//            request.SetRequestHeader("Authorization", "Bearer " + authToken);

//            yield return request.SendWebRequest();

//            if (request.result == UnityWebRequest.Result.Success)
//            {
//                authToken = request.downloadHandler.text;
//                tokenExpiryTime = Time.time + 1800; // 30 minutes
//                //Debug.Log("Token refreshed successfully!");
//            }
//            else
//            {
//                //Debug.LogError("Error renewing token: " + request.error);
//            }
//        }
//    }

//    //clear username
//    private void ClearUsername()
//    {
//        loggedInUsername = "";
//    }

//    public string GetAuthToken()
//    {
//        return authToken;
//    }

//    public string GetLoggedInUsername()
//    {
//        return loggedInUsername;
//    }

//}

