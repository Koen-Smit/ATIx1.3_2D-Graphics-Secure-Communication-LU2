using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExampleApp : MonoBehaviour
{
    [Header("Test data")]
    public User user;
    public Environment2D environment2D;
    public Object2DModel object2D;

    [Header("Dependencies")]
    public UserApiClient userApiClient;
    public Environment2DApiClient enviroment2DApiClient;
    public Object2DApiClient object2DApiClient;


    [Header("UI Elements")]
    public TextMeshProUGUI environmentListText;
    public TextMeshProUGUI objectListText;


    #region Login

    [ContextMenu("User/Register")]
    public async void Register()
    {
        IWebRequestReponse webRequestResponse = await userApiClient.Register(user);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                Debug.Log("Register success!");
                FindFirstObjectByType<LoginController>().ShowSuccessMessage("Registered!");
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Register error: " + errorMessage);
                FindFirstObjectByType<LoginController>().ShowSuccessMessage("Register failed: " + errorMessage);
                break;
        }
    }

    [ContextMenu("User/Login")]
    public async void Login()
    {
        IWebRequestReponse webRequestResponse = await userApiClient.Login(user);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                Debug.Log("Login success!");
                FindFirstObjectByType<LoginController>().ShowSuccessMessage("Logged in!");
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Login error: " + errorMessage);
                FindFirstObjectByType<LoginController>().ShowSuccessMessage("Login failed: " + errorMessage);
                break;
        }
    }



    #endregion

    #region Environment

    [ContextMenu("Environment2D/Read all")]
    public async void ReadEnvironment2Ds()
    {
        IWebRequestReponse webRequestResponse = await enviroment2DApiClient.ReadEnvironment2Ds();

        switch (webRequestResponse)
        {
            case WebRequestData<List<Environment2D>> dataResponse:
                environmentListText.text = "Environment IDs:\n";
                foreach (var env in dataResponse.Data)
                {
                    environmentListText.text += "- " + env.id + "\n";
                }
                Debug.Log("Fetched environment list successfully.");
                break;
            case WebRequestError errorResponse:
                environmentListText.text = "❌ Error fetching environments: " + errorResponse.ErrorMessage;
                Debug.Log("Read environment2Ds error: " + errorResponse.ErrorMessage);
                break;
        }
    }

    [ContextMenu("Environment2D/Create")]
    public async void CreateEnvironment2D()
    {
        IWebRequestReponse webRequestResponse = await enviroment2DApiClient.CreateEnvironment(environment2D);

        switch (webRequestResponse)
        {
            case WebRequestData<Environment2D> dataResponse:
                environment2D.id = dataResponse.Data.id;
                ReadEnvironment2Ds(); // Refresh the list
                break;
            case WebRequestError errorResponse:
                Debug.Log("Create environment2D error: " + errorResponse.ErrorMessage);
                break;
        }
    }

    [ContextMenu("Environment2D/Delete")]
    public async void DeleteEnvironment2D()
    {
        IWebRequestReponse webRequestResponse = await enviroment2DApiClient.DeleteEnvironment(environment2D.id);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                string responseData = dataResponse.Data;
                // TODO: Handle succes scenario.
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Delete environment error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    #endregion Environment

    #region Object2DModel

    [ContextMenu("Object2D/Read all")]
    public async void ReadObject2Ds()
    {
        IWebRequestReponse webRequestResponse = await object2DApiClient.ReadObject2Ds(object2D.environmentId);

        switch (webRequestResponse)
        {
            case WebRequestData<List<Object2DModel>> dataResponse:
                objectListText.text = "Objects IDs:\n";
                foreach (var obj in dataResponse.Data)
                {
                    objectListText.text += "- " + obj.id + "\n";
                }
                Debug.Log("Fetched object list successfully.");
                break;
            case WebRequestError errorResponse:
                objectListText.text = "❌ Error fetching objects: " + errorResponse.ErrorMessage;
                Debug.Log("Read object2Ds error: " + errorResponse.ErrorMessage);
                break;
        }
    }

    [ContextMenu("Object2D/Create")]
    public async void CreateObject2D()
    {
        IWebRequestReponse webRequestResponse = await object2DApiClient.CreateObject2D(object2D);

        switch (webRequestResponse)
        {
            case WebRequestData<Object2DModel> dataResponse:
                object2D.id = dataResponse.Data.id;
                ReadObject2Ds(); // Refresh the list
                break;
            case WebRequestError errorResponse:
                Debug.Log("Create Object2D error: " + errorResponse.ErrorMessage);
                break;
        }
    }

    [ContextMenu("Object2D/Update")]
    public async void UpdateObject2D()
    {
        IWebRequestReponse webRequestResponse = await object2DApiClient.UpdateObject2D(object2D);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                string responseData = dataResponse.Data;
                // TODO: Handle succes scenario.
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Update object2D error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    #endregion

}
