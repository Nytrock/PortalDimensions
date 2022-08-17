using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonFunctional : MonoBehaviour
{
    public Animator animator;

    [Header("�������������� � �������")]
    public Choice settingsChoice;
    public Choice mainChoice;
    public GameObject settingsPanel;
    public GameObject mainPanel;
    public Scrollbar gameSettingsSlider;

    [Header("��� ���� ��������")]
    public GameSettingsManager gameSettings;
    public ControllSettingsManager controllSettings;
    public VideoSettingsManager videoSettings;
    public AudioSettingsManager audioSettings;

    [Header("���� �������������")]
    public Button Yes;
    public Button No;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (animator.GetBool("isGameReset"))
                gameSettings.SetResetAnimation();
            else if (animator.GetBool("isMore"))
                gameSettings.SetMoreAnimation();
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
        }
    }
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

        mainPanel.SetActive(!animator.GetBool("isSettings"));
        settingsPanel.SetActive(animator.GetBool("isSettings"));

        if (animator.GetBool("isSettings")) {
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
        Application.Quit();
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
}
