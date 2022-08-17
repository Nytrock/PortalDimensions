using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseButtons : MonoBehaviour
{
    public CanvasManager canvas;
    public GameObject messageSettings;
    public GameObject messageExit;
    public GameSettingsManager settings;
    public Choice mainChoice;
    public Choice settingsChoice;

    public Button Yes;
    public Button No;

    public void ContinueGame()
    {
        canvas.SetPause();
    }

    public void Achievements()
    {
        Debug.Log("Achievements");
    }

    public void Statistics()
    {
        Debug.Log("Statistic");
    }

    public void ExitToMenuConfirm()
    {
        if (Save.GetConfirmNeed()) {
            Yes.onClick.RemoveAllListeners();
            Yes.onClick.AddListener(ExitToMenu);

            No.onClick.RemoveAllListeners();
            No.onClick.AddListener(canvas.SetConfirm);

            messageSettings.SetActive(false);
            messageExit.SetActive(true);

            canvas.SetConfirm();
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
            No.onClick.AddListener(canvas.SetConfirm);

            messageSettings.SetActive(false);
            messageExit.SetActive(true);

            canvas.SetConfirm();
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
}
