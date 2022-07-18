using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class ButtonFunctional : MonoBehaviour
{
    public Animator animator;
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Statistics()
    {
        Debug.Log("Statistic");
    }

    public void Settings()
    {
        animator.SetBool("isSettings", !animator.GetBool("isSettings"));
    }

    public void About()
    {
        Debug.Log("About");
    }

    public void Achievements()
    {
        Debug.Log("Achievements");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
