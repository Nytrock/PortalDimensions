using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class ButtonFunctional : MonoBehaviour
{
    public Animator animator;
    public void StartGame()
    {
        if (GameObject.Find("Save").GetComponent<Save>().SaveFile != null) {
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
}
