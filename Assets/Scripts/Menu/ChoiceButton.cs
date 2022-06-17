using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChoiceButton : MonoBehaviour,
    IPointerDownHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public Choice choice;
    public int id;

    public void OnPointerDown(PointerEventData eventData) {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        choice.GetPosition(id);
    }

    public void OnPointerExit(PointerEventData eventData) { 

    }
}
