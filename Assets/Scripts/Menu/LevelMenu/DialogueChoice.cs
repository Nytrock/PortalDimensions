using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueChoice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int NextPanel;
    public DialogueManager dialogueManager;

    public void ChangePanel()
    {
        dialogueManager.ChangePanel(NextPanel);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DialogueManager.isButton = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DialogueManager.isButton = false;
    }
}
