using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class ButtonFunctional : MonoBehaviour
{
    public Animator animator;
    public Choice MainChoice;
    public void StartGame()
    {
        if (File.Exists(Application.dataPath + "/save/PortalDimensionsSave.txt")) {
            Debug.Log("Вы уверены?");
        } else {
            SceneManager.LoadScene(1);
        }
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Settings()
    {
        animator.SetBool("Settings", !animator.GetBool("Settings"));
    }

    public void About()
    {
        Debug.Log("About");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ChoiceEnabled()
    {
        MainChoice.enabled = !MainChoice.enabled;
    }
}
