using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonFunctional : MonoBehaviour
{
    public static bool isGamePaused;
    public static bool pauseEnable;
    private Animator animator;

    [Header("Взаимодействие с выбором")]
    public Choice settingsChoice;
    public Choice mainChoice;
    public GameObject settingsPanel;
    public GameObject settingsMainPanel;
    public GameObject mainPanel;
    public Scrollbar gameSettingsSlider;

    [Header("Все меню настроек")]
    public GameSettingsManager gameSettings;
    public ControllSettingsManager controllSettings;
    public VideoSettingsManager videoSettings;
    public AudioSettingsManager audioSettings;

    [Header("Меню подтверждения")]
    public Button Yes;
    public Button No;
    public GameObject exitText;
    public GameObject settingsText;

    private void Start()
    {
        animator = GetComponent<Animator>();
        Time.timeScale = 1;
        pauseEnable = true;

        if (exitText != null) {
            exitText.SetActive(false);
            settingsText.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && pauseEnable) {
            if (animator.GetBool("isGameReset"))
                gameSettings.SetResetAnimation();
            else if (animator.GetBool("isMore"))
                gameSettings.SetMoreAnimation();
            else if (animator.GetBool("isConfirm"))
                SetConfirm();
            else if (animator.GetBool("isSettingsGame"))
                gameSettings.CheckChanges();
            else if (animator.GetBool("isSettingsControll"))
                controllSettings.CheckChanges();
            else if (animator.GetBool("isSettingsVideo"))
                videoSettings.CheckChanges();
            else if (animator.GetBool("isSettingsAudio"))
                audioSettings.CheckChanges();
            else if (animator.GetBool("isSettings"))
                Settings();
            else if (!isGamePaused && Save.save.dialogueChoiceManager != null)
                PauseGame();
            else if (isGamePaused)
                ResumeGame();
        }
    }
    public void StartGame()
    {
        SceneManager.LoadScene(2); // дебил, если забудешь поменять это в альфа-версии, то ты окончательно отупел
    }

    public void Statistics()
    {
        Debug.Log("Statistic");
    }

    public void Settings()
    {
        animator.SetBool("isSettings", !animator.GetBool("isSettings"));
        bool settingsActive = animator.GetBool("isSettings");

        settingsMainPanel.SetActive(!settingsActive);
        settingsPanel.SetActive(settingsActive);

        if (exitText != null) {
            exitText.SetActive(!settingsActive);
            settingsText.SetActive(settingsActive);
        }

        if (settingsActive) {
            settingsChoice.TargetPosition = settingsChoice.positions[0];
            settingsChoice.NowPosition = settingsChoice.positions[0];
            settingsChoice.transform.localPosition = new Vector2(settingsChoice.transform.localPosition.x, settingsChoice.positions[0]);
            settingsChoice.NowId = 0;
        }
    }

    public void SettingsWorking()
    {
        settingsChoice.working = animator.GetBool("isSettings");
    }

    public void MainWorking()
    {
        mainChoice.working = !animator.GetBool("isSettings");
    }

    public void GameSettings()
    {
        animator.SetBool("isSettingsGame", !animator.GetBool("isSettingsGame"));
        settingsChoice.working = !animator.GetBool("isSettingsGame");
        settingsPanel.SetActive(!animator.GetBool("isSettingsGame"));
        if (animator.GetBool("isSettingsGame"))
            gameSettingsSlider.value = 1;
    }

    public void ControllSettings()
    {
        animator.SetBool("isSettingsControll", !animator.GetBool("isSettingsControll"));
        settingsChoice.working = !animator.GetBool("isSettingsControll");
        settingsPanel.SetActive(!animator.GetBool("isSettingsControll"));
    }

    public void VideoSettings()
    {
        animator.SetBool("isSettingsVideo", !animator.GetBool("isSettingsVideo"));
        settingsChoice.working = !animator.GetBool("isSettingsVideo");
        settingsPanel.SetActive(!animator.GetBool("isSettingsVideo"));
    }

    public void AudioSettings()
    {
        animator.SetBool("isSettingsAudio", !animator.GetBool("isSettingsAudio"));
        settingsChoice.working = !animator.GetBool("isSettingsAudio");
        settingsPanel.SetActive(!animator.GetBool("isSettingsAudio"));
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
        Time.timeScale = 1;
        Application.Quit();
    }
    public void ExitToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(2);
    }

    public void SetConfirmPanel(string typeOfConfirm)
    {
        animator.SetBool("isConfirm", true);

        Yes.onClick.RemoveAllListeners();
        No.onClick.RemoveAllListeners();

        switch (typeOfConfirm) {
            case "GameSettings":
                Yes.onClick.AddListener(delegate { gameSettings.ConfirmCancel(true); gameSettings.SetChangesFalse(); GameSettings(); });
                No.onClick.AddListener(delegate { gameSettings.ConfirmCancel(false); });
                break;
            case "ControllSettings":
                Yes.onClick.AddListener(delegate { controllSettings.ConfirmCancel(true); controllSettings.SetChangesFalse(); ControllSettings(); });
                No.onClick.AddListener(delegate { controllSettings.ConfirmCancel(false); }); 
                break;
            case "VideoSettings":
                Yes.onClick.AddListener(delegate { videoSettings.ConfirmCancel(true); videoSettings.SetChangesFalse(); VideoSettings(); });
                No.onClick.AddListener(delegate { videoSettings.ConfirmCancel(false); });
                break;
            case "AudioSettings":
                Yes.onClick.AddListener(delegate { audioSettings.ConfirmCancel(true); audioSettings.SetChangesFalse(); AudioSettings(); });
                No.onClick.AddListener(delegate { audioSettings.ConfirmCancel(false); });
                break;
        }
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
        isGamePaused = true;
        mainChoice.StartPauseWorking();
        settingsChoice.StartPauseWorking();
        animator.SetBool("isPause", true);
        settingsMainPanel.SetActive(true);
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        isGamePaused = false;
        mainChoice.StopPauseWorking();
        settingsChoice.StopPauseWorking();
        animator.SetBool("isPause", false);
        settingsMainPanel.SetActive(false);
    }
    public void SetChoicePosition()
    {
        mainChoice.transform.localPosition = new Vector2(mainChoice.transform.localPosition.x, mainChoice.positions[0]);
        mainChoice.SetPosition(0);
        mainChoice.NowPosition = mainChoice.positions[0];
    }

    public void SetConfirm()
    {
        animator.SetBool("isConfirm", !animator.GetBool("isConfirm"));
    }
    public void SetActiveSettingsChoice()
    {
        settingsChoice.working = animator.GetBool("isSettings");
        settingsPanel.SetActive(animator.GetBool("isSettings"));
    }
    public void SetValue()
    {
        gameSettingsSlider.value = 1;
    }

    public void ActivateCoins()
    {
        animator.SetBool("isMoney", true);
        LevelManager.levelManager.StartParticles();
    }

    public void TurnOffMainButtons()
    {
        foreach (Button button in mainPanel.GetComponentsInChildren<Button>())
            button.interactable = false;
        pauseEnable = false;
    }
}
