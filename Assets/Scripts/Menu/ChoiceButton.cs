using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChoiceButton : MonoBehaviour,
    IPointerEnterHandler
{
    public Choice choice;
    public int id;

    public void OnPointerEnter(PointerEventData eventData)
    {
        choice.GetPosition(id);
    }
}
