using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueChoice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int NextPanel;
    public DialogueManager dialogueManager;
    public int id;

    public void ChangePanel()
    {
        dialogueManager.ChangePanel(NextPanel);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DialogueManager.isButton = true;
        var choice = dialogueManager.choiceArrow;
        choice.GetPosition(id);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DialogueManager.isButton = false;
    }
}
