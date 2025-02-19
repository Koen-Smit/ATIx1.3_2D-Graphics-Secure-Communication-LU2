using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{

    public GameManager gameManager;
    public HomeScreen homeScreen;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            gameManager.Play("rock");
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            gameManager.Play("paper");
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            gameManager.Play("scissors");
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Resetting game");
            gameManager.ResetGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Terug naar homescreen");
            SceneManager.LoadScene("HomeScreen");

        }
    }

}