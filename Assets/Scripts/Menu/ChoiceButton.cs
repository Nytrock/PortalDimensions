using UnityEngine;
using UnityEngine.EventSystems;

public class ChoiceButton : MonoBehaviour, IPointerEnterHandler
{
    public Choice choice;
    public int id;
    public bool pauseActive;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (pauseActive)
            choice.SetPosition(id);
    }
}
