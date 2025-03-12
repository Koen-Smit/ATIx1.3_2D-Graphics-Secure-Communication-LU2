using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class APIManager : MonoBehaviour
{
    private static APIManager _instance;
    private string baseUrl = "https://avansict2227609.azurewebsites.net";
    private string authToken = "";

    public static APIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("APIManager");
                _instance = obj.AddComponent<APIManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    private void Awake()
    {
        authToken = PlayerPrefs.GetString("authToken", "");
    }

    public void SetAuthToken(string token)
    {
        authToken = token;
        PlayerPrefs.SetString("authToken", token);
        PlayerPrefs.Save();
    }

    public void ClearAuthToken()
    {
        authToken = "";
        PlayerPrefs.DeleteKey("authToken");
        PlayerPrefs.Save();
    }

    private void SetupRequest(UnityWebRequest request, string jsonData)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        if (!string.IsNullOrEmpty(authToken))
        {
            request.SetRequestHeader("Authorization", "Bearer " + authToken);
        }
    }

    private void ResponseHandling(UnityWebRequest request, Action<APIResponse> callback)
    {
        if (callback == null)
        {
            Debug.LogError("Callback is null, skipping response handling.");
            return;
        }

        int statusCode = (int)request.responseCode;
        if (request.result != UnityWebRequest.Result.Success)
        {
            APIResponse response = new APIResponse(false, "API Error: " + request.error, null, statusCode);
            callback?.Invoke(response);

            if (statusCode == 401 || statusCode == 403) // Unauthorized, token is invalid
            {
                ClearAuthToken();
                SceneManager.LoadScene(0);
            }
        }
        else
        {
            APIResponse response = new APIResponse(true, "Request Successful", request.downloadHandler?.text, statusCode);
            callback?.Invoke(response);
        }
    }

    // REQUESTS
    public void GetRequest(string endpoint, Action<APIResponse> callback)
    {
        StartCoroutine(GetRequestCoroutine(endpoint, callback));
    }

    public void PostRequest(string endpoint, string jsonData, Action<APIResponse> callback)
    {
        StartCoroutine(PostRequestCoroutine(endpoint, jsonData, callback));
    }

    public void PutRequest(string endpoint, string jsonData, Action<APIResponse> callback)
    {
        StartCoroutine(PutRequestCoroutine(endpoint, jsonData, callback));
    }

    public void DeleteRequest(string endpoint, Action<APIResponse> callback)
    {
        StartCoroutine(DeleteRequestCoroutine(endpoint, callback));
    }

    // COROUTINES
    private IEnumerator GetRequestCoroutine(string endpoint, Action<APIResponse> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(baseUrl + endpoint))
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                request.SetRequestHeader("Authorization", "Bearer " + authToken);
            }

            yield return request.SendWebRequest();
            ResponseHandling(request, callback);
        }
    }

    private IEnumerator PostRequestCoroutine(string endpoint, string jsonData, Action<APIResponse> callback)
    {
        using (UnityWebRequest request = new UnityWebRequest(baseUrl + endpoint, "POST"))
        {
            SetupRequest(request, jsonData);
            yield return request.SendWebRequest();
            ResponseHandling(request, callback);
        }
    }

    private IEnumerator PutRequestCoroutine(string endpoint, string jsonData, Action<APIResponse> callback)
    {
        using (UnityWebRequest request = new UnityWebRequest(baseUrl + endpoint, "PUT"))
        {
            SetupRequest(request, jsonData);
            yield return request.SendWebRequest();
            ResponseHandling(request, callback);
        }
    }

    private IEnumerator DeleteRequestCoroutine(string endpoint, Action<APIResponse> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Delete(baseUrl + endpoint))
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                request.SetRequestHeader("Authorization", "Bearer " + authToken);
            }

            yield return request.SendWebRequest();
            ResponseHandling(request, callback);
        }
    }
}
