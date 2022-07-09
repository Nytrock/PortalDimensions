using UnityEngine;
using UnityEngine.UI;

public class LanguageButtons : MonoBehaviour
{
    public LocalizationManager localization;
    public Button PreviousButton;
    public Button NextButton;

    public void Start()
    {
        if (LocalizationManager.SelectedLanguage + 1 >= Save.NumberLanguages)
            NextButton.interactable = false;
        if (LocalizationManager.SelectedLanguage - 1 < 0)
            PreviousButton.interactable = false;
    }
    public void NextLanguage()
    {
        localization.SetLanguage(LocalizationManager.SelectedLanguage + 1);
        if (LocalizationManager.SelectedLanguage + 1 >= Save.NumberLanguages)
            NextButton.interactable = false;
        PreviousButton.interactable = true;
    }

    public void PreviousLanguage()
    {
        localization.SetLanguage(LocalizationManager.SelectedLanguage - 1);
        if (LocalizationManager.SelectedLanguage - 1 < 0)
            PreviousButton.interactable = false;
        NextButton.interactable = true;
    }
}
