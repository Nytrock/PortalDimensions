using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class ButtonFunctional : MonoBehaviour
{
    public Animator animator;

    [Header("Все меню настроек")]
    public GameSettingsManager gameSettings;
    public ControllSettingsManager controllSettings;
    public VideoSettingsManager videoSettings;
    public AudioSettingsManager audioSettings;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (animator.GetBool("isSettingsGame"))
                gameSettings.CheckChanges();
            else if (animator.GetBool("isSettingsControll"))
                controllSettings.CheckChanges();
            else if (animator.GetBool("isSettingsVideo"))
                videoSettings.CheckChanges();
            else if (animator.GetBool("isSettingsAudio"))
                audioSettings.CheckChanges();
            else if (animator.GetBool("isSettings"))
                Settings();
        }
    }
    public void StartGame()
    {
        if (!animator.GetBool("isSettings"))
            SceneManager.LoadScene(1);
    }

    public void Statistics()
    {
        if (!animator.GetBool("isSettings"))
            Debug.Log("Statistic");
    }

    public void Settings()
    {
        animator.SetBool("isSettings", !animator.GetBool("isSettings"));
    }

    public void GameSettings()
    {
        animator.SetBool("isSettingsGame", !animator.GetBool("isSettingsGame"));
    }

    public void ControllSettings()
    {
        animator.SetBool("isSettingsControll", !animator.GetBool("isSettingsControll"));
    }

    public void VideoSettings()
    {
        animator.SetBool("isSettingsVideo", !animator.GetBool("isSettingsVideo"));
    }

    public void AudioSettings()
    {
        animator.SetBool("isSettingsAudio", !animator.GetBool("isSettingsAudio"));
    }

    public void About()
    {
        if (!animator.GetBool("isSettings"))
            Debug.Log("About");
    }

    public void Achievements()
    {
        if (!animator.GetBool("isSettings"))
            Debug.Log("Achievements");
    }

    public void Exit()
    {
        if (!animator.GetBool("isSettings"))
            Application.Quit();
    }
}
