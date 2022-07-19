using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseButtons : MonoBehaviour
{
    public Animator canvas;
    public Choice choice;
    public GameObject messageSettings;
    public GameObject messageExit;
    public SettingsManager settings;

    public Button Yes;
    public Button No;

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
        Yes.onClick.RemoveAllListeners();
        Yes.onClick.AddListener(delegate { settings.ConfirmCancel(true); });

        No.onClick.RemoveAllListeners();
        No.onClick.AddListener(delegate { settings.ConfirmCancel(false); });

        messageSettings.SetActive(true);
        messageExit.SetActive(false);

        canvas.SetBool("isSettings", !canvas.GetBool("isSettings"));
    }

    public void ExitToMenuConfirm()
    {
        if (Save.GetConfirmNeed()) {
            Yes.onClick.RemoveAllListeners();
            Yes.onClick.AddListener(ExitToMenu);

            No.onClick.RemoveAllListeners();
            No.onClick.AddListener(CloseConfirm);

            messageSettings.SetActive(false);
            messageExit.SetActive(true);

            canvas.SetBool("isConfirm", true);
        } else {
            ExitToMenu();
        }
    }

    public void ExitConfirm()
    {
        if (Save.GetConfirmNeed()) {
            Yes.onClick.RemoveAllListeners();
            Yes.onClick.AddListener(Exit);

            No.onClick.RemoveAllListeners();
            No.onClick.AddListener(CloseConfirm);

            messageSettings.SetActive(false);
            messageExit.SetActive(true);

            canvas.SetBool("isConfirm", true);
        } else {
            Exit();
        }
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

    public void CloseConfirm()
    {
        canvas.SetBool("isConfirm", false);
    }
}
