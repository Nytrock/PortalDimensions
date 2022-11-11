using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DialogueChoice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int NextPanel;
    public DialogueManager dialogueManager;
    public int id;
    public int doId = -1;
    public int existingId = -1;

    public void ChangePanel()
    {
        StartCoroutine(WaitAndChange());
    }
    
    IEnumerator WaitAndChange()
    {
        yield return new WaitForSeconds(Time.deltaTime * 5);
        dialogueManager.ChangePanel(NextPanel);
        if (doId > -1) {
            dialogueManager.choiceManager.DoSomethingFromId(doId);
            dialogueManager.save.SetChoiceDoing(doId, true);
        }
        if (existingId > -1)
            dialogueManager.save.SetChoiceExisting(existingId, false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DialogueManager.isButton = true;
        var choice = dialogueManager.choiceArrow;
        choice.SetPosition(id);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DialogueManager.isButton = false;
    }
}
