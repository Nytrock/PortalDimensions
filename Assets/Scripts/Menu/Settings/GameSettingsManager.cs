using UnityEngine;
using UnityEngine.UI;

public class GameSettingsManager : MonoBehaviour
{
    public Animator canvas;

    [Header("Дефолтные настройки")]
    [SerializeField] private int defaultLanguage;
    [SerializeField] private bool defaultAuto;
    [SerializeField] private bool defaultFps;
    [SerializeField] private bool defaultConfirm;

    [Header("Настройки локализации")]
    public LocalizationManager localization;
    public int originallyLanguage;
    public Button PreviousButton;
    public Button NextButton;
    private bool isLanguageChange;

    [Header("Настройки авторестарта")]
    public Toggle autoManager;
    public bool autorestart;
    private bool originallyAuto;
    private bool isAutoChange;

    [Header("Настройки счётчика fps")]
    public Toggle fpsManager;
    public FpsCounter fpsCounter;
    public bool fpsShowing;
    private bool originallyFps;
    private bool isFpsChange;

    [Header("Настройки подтверждения выхода")]
    public Toggle confirmManager;
    public bool confirm;
    private bool originallyConfirm;
    private bool isConfirmChange;

    public void Start()
    {
        NextButton.interactable = !(LocalizationManager.SelectedLanguage + 1 >= Save.NumberLanguages);
        PreviousButton.interactable = !(LocalizationManager.SelectedLanguage - 1 < 0);

        SetNewOriginall();
        SetChangesFalse();

        autorestart = originallyAuto;
        confirm = originallyConfirm;
        fpsShowing = originallyFps;
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
        autorestart = !autorestart;
        isAutoChange = autorestart != originallyAuto;
    }

    public void ChangeConfirmNeed()
    {
        confirm = !confirm;
        isConfirmChange = confirm != originallyConfirm;
    }
    public void ChangeFps()
    {
        fpsShowing = !fpsShowing;
        isFpsChange = fpsShowing !=  originallyFps;
        fpsCounter.ChangeWorking(fpsShowing);
    }

    public void CheckChanges()
    {
        if (isLanguageChange || isAutoChange || isFpsChange || isConfirmChange) {
            canvas.GetComponent<ButtonFunctional>().SetConfirmPanel("GameSettings");
        } else {
            canvas.SetBool("isSettingsGame", !canvas.GetBool("isSettingsGame"));
        }
    }
    public void ConfirmCancel(bool value)
    {
        if (value)
            ReturnToNormal();
        canvas.SetBool("isConfirm", false);
    }

    private void ReturnToNormal()
    {
        localization.SetLanguage(originallyLanguage);
        NextButton.interactable = !(LocalizationManager.SelectedLanguage + 1 >= Save.NumberLanguages);
        PreviousButton.interactable = !(LocalizationManager.SelectedLanguage - 1 < 0);
        isLanguageChange = isAutoChange = isFpsChange = isConfirmChange = false;

        fpsManager.isOn = originallyFps;
        fpsCounter.ChangeWorking(originallyFps);

        autoManager.isOn = originallyAuto;

        confirmManager.isOn = originallyConfirm;

        canvas.SetBool("isSettingsGame", false);
    }

    public void SetNewOriginall()
    {
        originallyAuto = autoManager.isOn;
        originallyConfirm = confirmManager.isOn;
        originallyFps = fpsManager.isOn;
        originallyLanguage = LocalizationManager.SelectedLanguage;
    }

    public void SetChangesFalse()
    {
        isAutoChange = false;
        isConfirmChange = false;
        isFpsChange = false;
    }

    public void SetDefaults()
    {
        localization.SetLanguage(defaultLanguage);
        PreviousButton.interactable = !(defaultLanguage - 1 < 0);
        NextButton.interactable = defaultLanguage + 1 < LocalizationManager.numLanguages;
        isLanguageChange = originallyLanguage != LocalizationManager.SelectedLanguage;

        autorestart = defaultAuto;
        autoManager.isOn = defaultAuto;
        isAutoChange = autorestart != originallyAuto;

        confirm = defaultConfirm;
        confirmManager.isOn = defaultConfirm;
        isConfirmChange = confirm != originallyConfirm;

        fpsShowing = defaultFps;
        isFpsChange = fpsShowing != originallyFps;
        fpsManager.isOn = defaultFps;
        fpsCounter.ChangeWorking(fpsShowing);
    }
}
