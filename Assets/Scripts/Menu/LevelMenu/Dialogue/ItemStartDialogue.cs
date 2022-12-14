using UnityEngine;
using TMPro;

public class ItemStartDialogue : MonoBehaviour
{
    public string KeyToDialogue;
    public bool InstantStart;

    private KeyCode startKey;

    private void Start()
    {
        SetControll();
        ControllSettingsManager.OnButtonChange += SetControll;

        if (!InstantStart) {
            GetComponentInChildren<LocalizedText>().Localize(GetComponentInChildren<TextMeshProUGUI>().text);
            AddKeyToText();
            LocalizationManager.OnLanguageChange += AddKeyToText;
        }
    }

    private void Update()
    {
        if (!InstantStart) {
            if (Input.GetKeyDown(startKey) && GetComponent<Animator>().GetBool("isActive") && !ButtonFunctional.isGamePaused) {
                GetComponent<Animator>().SetBool("isActive", false);
                DialogueManager.dialogueManager.SetKey(KeyToDialogue);
                DialogueManager.dialogueManager.StartDialogue();
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player _))
        {
            if (InstantStart) {
                DialogueManager.dialogueManager.SetKey(KeyToDialogue);
                DialogueManager.dialogueManager.StartDialogue();
            } else {
                StartAnimation();
            }
        }
    }

    public void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player _))
        {
            if (!InstantStart)
                GetComponent<Animator>().SetBool("isActive", false);
        }
    }

    public void StartAnimation()
    {
        GetComponent<Animator>().SetBool("isActive", true);
    }

    private void AddKeyToText()
    {
        GetComponentInChildren<TextMeshProUGUI>().text += " (" + startKey.ToString() + ")";
    }

    private void SetControll()
    {
        startKey = Save.save.dialogueStartKey;
    }
}
