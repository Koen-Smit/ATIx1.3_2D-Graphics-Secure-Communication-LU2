using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    [SerializeField] private TextMeshProUGUI username;

    void Start()
    {
        pauseMenuPanel.SetActive(false);
        APIManager.Instance.GetRequest("/account/username", OnUsernameReceived);
    }
    void OnUsernameReceived(APIResponse response)
    {
        if (!response.Success || response.StatusCode == 405)
            APIManager.Instance.PostRequest("/account/logout", "", OnLogoutResponse);
        else
            username.text = "Logged in as: " + response.Data;
    }

    public void TogglePauseMenu()
    {
        bool isActive = pauseMenuPanel.activeSelf;
        pauseMenuPanel.SetActive(!isActive);
    }

    public void OnLogoutButtonPressed()
    {
        APIManager.Instance.PostRequest("/account/logout", "", OnLogoutResponse);
    }
    void OnLogoutResponse(APIResponse response)
    {
        if (response.Success)
        {
            SceneManager.LoadScene(0);
        }
    }
}
