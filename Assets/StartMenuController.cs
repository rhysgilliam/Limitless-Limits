using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public void OnStartClick()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void OnExitClick()
    {
        Application.Quit();
    }
}
