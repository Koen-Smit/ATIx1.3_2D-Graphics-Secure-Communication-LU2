using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeScreen : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");

    }

    public void StopGame()
    {
        Debug.Log("Exiting application");
        Application.Quit();

    }
}
