using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public static bool isGamePaused;
    public LevelManager levelManager;

    [Header("Взаимодействие с выбором")]
    public Choice mainChoice;
    public Choice settingsChoice;
    public GameObject settingsPanel;
    public GameObject mainPanel;
    public Scrollbar gameSettingsSlider;

    private float choiceYCoordinate;
    private Animator animator;

    public void Start()
    {
        animator = GetComponent<Animator>();
        choiceYCoordinate = mainChoice.transform.localPosition.y;
        mainPanel.SetActive(animator.GetBool("isPause"));
        settingsPanel.SetActive(animator.GetBool("isSettings"));
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (animator.GetBool("isConfirm"))
                SetConfirm();
            else if (animator.GetBool("isSettings"))
                Settings();
            else
                SetPause();
        }
    }

    public void ReloadScene()
    {
        levelManager.RestartScene();
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
        isGamePaused = true;
        mainChoice.StartPauseWorking();
        settingsChoice.StartPauseWorking();
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        isGamePaused = false;
        mainChoice.StopPauseWorking();
        settingsChoice.StopPauseWorking();
    }

    public void SetChoicePosition()
    {
        mainChoice.transform.localPosition = new Vector2(mainChoice.transform.localPosition.x, choiceYCoordinate);
        mainChoice.SetPosition(0);
        mainChoice.NowPosition = mainChoice.positions[0];
    }

    public void Settings()
    {
        animator.SetBool("isSettings", !animator.GetBool("isSettings"));
        mainChoice.working = !animator.GetBool("isSettings");
        mainPanel.SetActive(!animator.GetBool("isSettings"));
    }

    public void SetActiveSettingsChoice()
    {
        settingsChoice.working = animator.GetBool("isSettings");
        settingsPanel.SetActive(animator.GetBool("isSettings"));
    }

    public void SetConfirm()
    {
        animator.SetBool("isConfirm", !animator.GetBool("isConfirm"));
    }

    public void SetPause()
    {
        animator.SetBool("isPause", !animator.GetBool("isPause"));
        mainPanel.SetActive(animator.GetBool("isPause"));
    }
    public void Restart()
    {
        animator.SetBool("isDeath", animator.GetBool("isDeath"));
    }

    public void GameSettings()
    {
        animator.SetBool("isSettingsGame", !animator.GetBool("isSettingsGame"));
        settingsChoice.working = !animator.GetBool("isSettingsGame");
        settingsPanel.SetActive(!animator.GetBool("isSettingsGame"));
        if (animator.GetBool("isSettingsGame"))
            gameSettingsSlider.value = 1;
    }

    public void SetValue()
    {
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
}
