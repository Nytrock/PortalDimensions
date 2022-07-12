using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public ButtonFunctional canvas;

    [Header("Настройки локализации")]
    public LocalizationManager localization;
    public int originallyLanguage;
    public Button PreviousButton;
    public Button NextButton;
    public bool isLanguageChange;

    [Header("Настройки авторестарта")]
    public Toggle autoManager;
    public bool Autorestart;
    public bool originallyAuto;
    public bool isAutoChange;

    public void Start()
    {
        NextButton.interactable = !(LocalizationManager.SelectedLanguage + 1 >= Save.NumberLanguages);
        PreviousButton.interactable = !(LocalizationManager.SelectedLanguage - 1 < 0);
        originallyLanguage = LocalizationManager.SelectedLanguage;
        originallyAuto = autoManager.isOn;
        Autorestart = originallyAuto;
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

    public void CheckChanges()
    {
        if (isLanguageChange || isAutoChange) {
            Debug.Log("Вы уверены?");
        } else {
            canvas.animator.SetBool("Settings", !canvas.animator.GetBool("Settings"));
        }
    }
}
