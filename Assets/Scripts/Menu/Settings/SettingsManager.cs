using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public bool isSettingsChange;
    public ButtonFunctional canvas;

    [Header("Настройки локализации")]
    public LocalizationManager localization;
    public int originallyLanguage;
    public Button PreviousButton;
    public Button NextButton;

    public void Start()
    {
        NextButton.interactable = !(LocalizationManager.SelectedLanguage + 1 >= Save.NumberLanguages);
        PreviousButton.interactable = !(LocalizationManager.SelectedLanguage - 1 < 0);
        originallyLanguage = LocalizationManager.SelectedLanguage;
    }
    public void NextLanguage()
    {
        localization.SetLanguage(LocalizationManager.SelectedLanguage + 1);
        NextButton.interactable = !(LocalizationManager.SelectedLanguage + 1 >= Save.NumberLanguages);
        PreviousButton.interactable = true;
        isSettingsChange = originallyLanguage != LocalizationManager.SelectedLanguage;
    }

    public void PreviousLanguage()
    {
        localization.SetLanguage(LocalizationManager.SelectedLanguage - 1);
        PreviousButton.interactable = !(LocalizationManager.SelectedLanguage - 1 < 0);
        NextButton.interactable = true;
        isSettingsChange = originallyLanguage != LocalizationManager.SelectedLanguage;
    }

    public void CheckChanges()
    {
        if (isSettingsChange) {
            Debug.Log("Вы уверены?");
        } else {
            canvas.animator.SetBool("Settings", !canvas.animator.GetBool("Settings"));
        }
    }
}
