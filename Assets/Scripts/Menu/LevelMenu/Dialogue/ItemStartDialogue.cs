using UnityEngine;
using TMPro;

public class ItemStartDialogue : MonoBehaviour
{
    public string KeyToDialogue;
    public bool InstantStart;
    public DialogueManager dialogueManager;

    private KeyCode startKey;

    private void Start()
    {
        startKey = Save.save.dialogueStartKey;
        GetComponentInChildren<LocalizedText>().Localize();
        AddKeyToText();
        LocalizationManager.OnLanguageChange += AddKeyToText;
    }

    private void Update()
    {
        if (!InstantStart) {
            if (Input.GetKeyDown(startKey) && GetComponent<Animator>().GetBool("isActive") && !ButtonFunctional.isGamePaused) {
                GetComponent<Animator>().SetBool("isActive", false);
                dialogueManager.SetKey(KeyToDialogue);
                dialogueManager.StartDialogue();
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player _))
        {
            if (InstantStart) {
                dialogueManager.SetKey(KeyToDialogue);
                dialogueManager.StartDialogue();
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
                this.GetComponent<Animator>().SetBool("isActive", false);
        }
    }

    public void StartAnimation()
    {
        this.GetComponent<Animator>().SetBool("isActive", true);
    }

    private void AddKeyToText()
    {
        GetComponentInChildren<TextMeshProUGUI>().text += "(" + startKey.ToString() + ")";
    }
}
