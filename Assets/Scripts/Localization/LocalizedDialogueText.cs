using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]

public class LocalizedDialogueText : MonoBehaviour
{
    private TextMeshProUGUI text;
    public string key;
    public DialogueManager dialogueManager;

    private void Start()
    {
        LocalizationManager.OnLanguageChange += OnLanguageChange;
    }

    private void OnLanguageChange()
    {
        Localize();
    }

    private void Init()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void OnDestroy()
    {
        LocalizationManager.OnLanguageChange -= OnLanguageChange;
    }

    public void Localize(string newKey = null)
    {
        if (text == null)
            Init();

        if (newKey != null)
            key = newKey;

        if (dialogueManager.isTextShow) {
            dialogueManager.NeedText = LocalizationManager.GetTranslate(key);
            dialogueManager.ChangeMainText();
        }
        else
            text.text = LocalizationManager.GetTranslate(key);
    }
}
