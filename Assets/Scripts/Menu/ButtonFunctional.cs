using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonFunctional : MonoBehaviour
{
    public Animator animator;
    public Choice settingsChoice;
    public Choice mainChoice;

    [Header("Все меню настроек")]
    public GameSettingsManager gameSettings;
    public ControllSettingsManager controllSettings;
    public VideoSettingsManager videoSettings;
    public AudioSettingsManager audioSettings;

    [Header("Меню подтверждения")]
    public Button Yes;
    public Button No;

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
        mainChoice.working = !animator.GetBool("isSettings");
        settingsChoice.working = animator.GetBool("isSettings");
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

    public void SetConfirmPanel(string typeOfConfirm)
    {
        animator.SetBool("isConfirm", true);

        Yes.onClick.RemoveAllListeners();
        No.onClick.RemoveAllListeners();

        switch (typeOfConfirm) {
            case "GameSettings":
                Yes.onClick.AddListener(delegate { gameSettings.ConfirmCancel(true); gameSettings.SetChangesFalse(); });
                No.onClick.AddListener(delegate { gameSettings.ConfirmCancel(false); });
                break;
            case "ControllSettings":
                Yes.onClick.AddListener(delegate { controllSettings.ConfirmCancel(true); controllSettings.SetChangesFalse(); });
                No.onClick.AddListener(delegate { controllSettings.ConfirmCancel(false); }); 
                break;
            case "VideoSettings":
                Yes.onClick.AddListener(delegate { videoSettings.ConfirmCancel(true); videoSettings.SetChangesFalse(); });
                No.onClick.AddListener(delegate { videoSettings.ConfirmCancel(false); });
                break;
            case "AudioSettings":
                Yes.onClick.AddListener(delegate { audioSettings.ConfirmCancel(true); audioSettings.SetChangesFalse(); });
                No.onClick.AddListener(delegate { audioSettings.ConfirmCancel(false); });
                break;
        }
    }

    public void SetSettingsChoice()
    {
        settingsChoice.TargetPosition = settingsChoice.positions[0];
        settingsChoice.NowPosition = settingsChoice.positions[0];
        settingsChoice.transform.localPosition = new Vector2(settingsChoice.transform.localPosition.x, settingsChoice.positions[0]);
        settingsChoice.NowId = 0;
    }
}
