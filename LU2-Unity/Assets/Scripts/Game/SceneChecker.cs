using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChecker : MonoBehaviour
{
    public void OnBackButtonPressed() => SceneManager.LoadScene("ShareSelector");
}
