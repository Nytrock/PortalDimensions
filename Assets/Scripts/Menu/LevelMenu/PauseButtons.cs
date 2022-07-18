using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtons : MonoBehaviour
{
    public Animator canvas;
    public Choice choice;

    public void ContinueGame()
    {
        canvas.SetBool("isPause", false);
    }

    public void Achievements()
    {
        Debug.Log("Achievements");
    }

    public void Statistics()
    {
        Debug.Log("Statistic");
    }

    public void Settings()
    {
        canvas.SetBool("isSettings", !canvas.GetBool("isSettings"));
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(2);
    }

    public void Exit()
    {
        Time.timeScale = 1;
        Application.Quit();
    }
}
