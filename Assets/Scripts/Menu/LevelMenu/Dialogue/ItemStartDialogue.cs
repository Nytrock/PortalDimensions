using UnityEngine;

public class ItemStartDialogue : MonoBehaviour
{
    public string KeyToDialogue;
    public bool InstantStart;
    public DialogueManager dialogueManager;

    public void Update()
    {
        if (!InstantStart) {
            if (Input.GetKeyDown(KeyCode.E) && this.GetComponent<Animator>().GetBool("isActive") && !CanvasManager.isGamePaused) {
                this.GetComponent<Animator>().SetBool("isActive", false);
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
            }
            else
                StartAnimation();
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
}
