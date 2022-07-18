using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Animator canvas;

    [Header("Настройки локализации")]
    public LocalizationManager localization;
    public int originallyLanguage;
    public Button PreviousButton;
    public Button NextButton;
    public bool isLanguageChange;

    [Header("Настройки авторестарта")]
    public Toggle autoManager;
    public bool Autorestart;
    private bool originallyAuto;
    private bool isAutoChange;

    [Header("Настройки счётчика fps")]
    public Toggle fpsManager;
    public FpsCounter fpsCounter;
    public bool fpsShowing;
    public bool originallyFps;
    public bool isFpsChange;

    public void Start()
    {
        NextButton.interactable = !(LocalizationManager.SelectedLanguage + 1 >= Save.NumberLanguages);
        PreviousButton.interactable = !(LocalizationManager.SelectedLanguage - 1 < 0);
        originallyLanguage = LocalizationManager.SelectedLanguage;

        originallyAuto = autoManager.isOn;
        Autorestart = originallyAuto;

        originallyFps = fpsManager.isOn;
        fpsShowing = originallyFps;
        isFpsChange = false;
    }
    public void NextLanguage()
    {
        localization.SetLanguage(LocalizationManager.SelectedLanguage + 1);
        NextButton.interactable = !(LocalizationManager.SelectedLanguage + 1 >= Save.NumberLanguages);
        PreviousButton.interactable = true;
        isLanguageChange = originallyLanguage != LocalizationManager.SelectedLanguage;
    }

    public void PreviousLanguage()
    {
        localization.SetLanguage(LocalizationManager.SelectedLanguage - 1);
        PreviousButton.interactable = !(LocalizationManager.SelectedLanguage - 1 < 0);
        NextButton.interactable = true;
        isLanguageChange = originallyLanguage != LocalizationManager.SelectedLanguage;
    }

    public void ChangeAutorestart()
    {
        Autorestart = !Autorestart;
        isAutoChange = Autorestart != originallyAuto;
    }
    public void ChangeFps()
    {
        fpsShowing = !fpsShowing;
        isFpsChange = fpsShowing !=  originallyFps;
        fpsCounter.ChangeWorking(fpsShowing);
    }

    public void CheckChanges()
    {
        if (isLanguageChange || isAutoChange || isFpsChange) {
            Debug.Log("Вы уверены?");
        } else {
            canvas.SetBool("isSettings", !canvas.GetBool("isSettings"));
        }
    }
}
